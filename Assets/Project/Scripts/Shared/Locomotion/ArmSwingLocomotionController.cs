using UnityEngine;
using System.Collections.Generic;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace JGVR.Locomotion
{
    public class ArmSwingLocomotionController : MonoBehaviour
    {
        public enum ControlOptions
        {
            HMDAndControllers, // Track both HMD and controllers        
            ControllersOnly,   // Track only the controllers
            HMDOnly,           // Track only HMD
        }

        public enum DirectionalMethod
        {
            Gaze,                           // Move in the direction of the HMD
            ControllerRotation,             // Move in the averaged direction of the controllers
            DumbDecoupling,                 // Move in the direction the HMD had when the activate button was pressed
            SmartDecoupling,                // Move in the direction of the HMD only if it points the same direction as the controllers
            ActivateControllerRotationOnly, // Move in the direction of the controller where the activate button is pressed
            LeftControllerRotationOnly,     // Move in the direction of the left controller
            RightControllerRotationOnly     // Move in the direction of the right controller
        }

        [Header("Control Settings")]

        [SerializeField] private Transform _hmd;
        [SerializeField] private Hand _controllerLeftHand;
        [SerializeField] private Hand _controllerRightHand;
        [SerializeField] private Transform _playArea;

        [SerializeField] private bool _useLeftHandActivation = true;
        [SerializeField] private bool _useRightHandActivation = true;

        [Tooltip("The button used to activate movement")]
        [SerializeField] private SteamVR_Action_Boolean _activateButton;

        [Tooltip("The device used to determine the movement paramters")]
        [SerializeField] private ControlOptions _controlOptions = ControlOptions.HMDAndControllers;

        [Tooltip("The method used to determine the direction of forward movement")]
        [SerializeField] private DirectionalMethod _directionMethod = DirectionalMethod.Gaze;

        [Header("Speed Settings")]

        [Tooltip("The speed with which to move the play area")]
        [SerializeField] private float _speedScale = 1;

        [Tooltip("The maximun speed in game units (if 0 or less, max speed is uncapped)")]
        [SerializeField] private float _maxSpeed = 4;

        [Tooltip("The speed with which movement slows down to a complete stop when the activate button is released. This deceleration effect can ease motion sickness")]
        [SerializeField] private float _deceleration = 0.1f;

        [Header("Advanced Settings")]

        [Tooltip("The degree threshold that tracked objects (controllers, HMD) must be within in order to change the movement direction when using the SmartDecoupling direction method")]
        [SerializeField] private float _smartDecoupleThreshold = 30f;

        [Tooltip("The maximum amount of movement registered. Decreasing this will increase acceleration")]
        [SerializeField] private float _sensitivity = 0.02f;

        private SteamVR_Input_Sources _engagedController;
        private Transform _engagedControllerTransform;
        private SteamVR_Action_Boolean _previousActivateButton;
        private int _averagePeriod = 60;
        private List<Transform> _trackedObjects = new List<Transform>();
        private Dictionary<Transform, List<float>> _movementList = new Dictionary<Transform, List<float>>();
        private Dictionary<Transform, float> _previousYPositions = new Dictionary<Transform, float>();
        private Vector3 _initialGaze;
        private float _currentSpeed;
        private Vector3 _currentDirection;
        private Vector3 _previousDirection;
        private bool _movementEngaged;

        private void OnEnable()
        {
            _trackedObjects.Clear();
            _movementList.Clear();
            _previousYPositions.Clear();
            _initialGaze = Vector3.zero;
            _currentDirection = Vector3.zero;
            _previousDirection = Vector3.zero;
            _currentSpeed = 0f;
            _movementEngaged = false;
            _previousActivateButton = _activateButton;

            SetControlOptions(_controlOptions);

            // Initialize lists
            for (int i = 0; i < _trackedObjects.Count; i++)
            {
                var trackedObj = _trackedObjects[i];

                _movementList.Add(trackedObj, new List<float>());
                _previousYPositions.Add(trackedObj, trackedObj.transform.localPosition.y);
            }

            if (_activateButton != null)
            {
                _activateButton.AddOnChangeListener(OnTriggerPressedOrReleased, SteamVR_Input_Sources.Any);
            }
        }

        private void OnDisable()
        {
            if (_activateButton != null)
            {
                _activateButton.RemoveOnChangeListener(OnTriggerPressedOrReleased, SteamVR_Input_Sources.Any);
            }
        }

        private void Update()
        {
            _previousActivateButton = _activateButton;
        }

        private void FixedUpdate()
        {
            if (MovementActivated())
            {
                // Initialize the list average
                var speed = Mathf.Clamp(((_speedScale * 350f) * (calculateListAverage() / _trackedObjects.Count)), 0f, _maxSpeed);
                _previousDirection = _currentDirection;
                _currentDirection = calculateDirection();

                // Update current speed
                _currentSpeed = speed;
            }
            else if (_currentSpeed > 0f)
            {
                _currentSpeed -= _deceleration;
            }
            else
            {
                _currentSpeed = 0f;
                _currentDirection = Vector3.zero;
                _previousDirection = Vector3.zero;
            }

            setDeltaTransformData();
            movePlayArea(_currentDirection, _currentSpeed);
        }

        // Set the control options and modify the trackables to match
        public void SetControlOptions(ControlOptions givenControlOptions)
        {
            _controlOptions = givenControlOptions;
            _trackedObjects.Clear();

            if (_controllerLeftHand != null && _controllerRightHand != null && (_controlOptions == ControlOptions.HMDAndControllers || _controlOptions == ControlOptions.ControllersOnly))
            {
                _trackedObjects.Add(_controllerLeftHand.transform);
                _trackedObjects.Add(_controllerRightHand.transform);
            }

            if (_hmd != null && (_controlOptions == ControlOptions.HMDAndControllers || _controlOptions == ControlOptions.HMDOnly))
            {
                _trackedObjects.Add(_hmd);
            }
        }

        public Vector3 GetMovementDirection()
        {
            return _currentDirection;
        }

        public float GetSpeed()
        {
            return _currentSpeed;
        }

        public bool MovementActivated()
        {
            return (_movementEngaged || _activateButton == null);
        }

        private float calculateListAverage()
        {
            var listAverage = 0f;

            for (int i = 0; i < _trackedObjects.Count; i++)
            {
                var trackedObj = _trackedObjects[i];

                // Get the amount of Y movement that's occured since the last update
                var previousYPosition = _previousYPositions[trackedObj];
                var deltaYPostion = Mathf.Abs(previousYPosition - trackedObj.transform.localPosition.y);

                var trackedObjList = _movementList[trackedObj];

                // Cap off the speed
                if (deltaYPostion > _sensitivity)
                {
                    trackedObjList.Add(_sensitivity);
                }
                else
                {
                    trackedObjList.Add(deltaYPostion);
                }

                // Keep our tracking list at _averagePeriod number of elements
                if (trackedObjList.Count > _averagePeriod)
                {
                    trackedObjList.RemoveAt(0);
                }

                // Average out the current tracker's list
                var sum = 0f;
                for (int j = 0; j < trackedObjList.Count; j++)
                {
                    var diffrences = trackedObjList[j];
                    sum += diffrences;
                }
                var avg = sum / _averagePeriod;

                // Add the average to the the list average
                listAverage += avg;
            }

            return listAverage;
        }

        private Vector3 hmdPosition()
        {
            return (_hmd != null ? new Vector3(_hmd.forward.x, 0, _hmd.forward.z) : Vector3.zero);
        }

        private Vector3 calculateDirection()
        {
            switch (_directionMethod)
            {
                case DirectionalMethod.SmartDecoupling:
                case DirectionalMethod.DumbDecoupling:
                    return calculateCouplingDirection();
                case DirectionalMethod.ControllerRotation:
                    return calculateControllerRotationDirection(determineAverageControllerRotation() * Vector3.forward);
                case DirectionalMethod.LeftControllerRotationOnly:
                    return calculateControllerRotationDirection((_controllerLeftHand != null ? _controllerLeftHand.transform.rotation : Quaternion.identity) * Vector3.forward);
                case DirectionalMethod.RightControllerRotationOnly:
                    return calculateControllerRotationDirection((_controllerRightHand != null ? _controllerRightHand.transform.rotation : Quaternion.identity) * Vector3.forward);
                case DirectionalMethod.ActivateControllerRotationOnly:
                    return calculateControllerRotationDirection(_engagedControllerTransform.rotation * Vector3.forward);
                case DirectionalMethod.Gaze:
                    return hmdPosition();
            }

            return Vector3.zero;
        }

        private Vector3 calculateCouplingDirection()
        {
            // If we haven't set an inital gaze yet
            // For DumbDecoupling this is all we need
            if (_initialGaze == Vector3.zero)
            {
                _initialGaze = hmdPosition();
            }

            // For SmartDecoupling check to see if we need to reset our distance
            if (_directionMethod == DirectionalMethod.SmartDecoupling)
            {
                var closeEnough = true;
                var curXDirection = (_hmd != null ? _hmd.rotation.eulerAngles.y : 0f);
                if (curXDirection <= _smartDecoupleThreshold)
                {
                    curXDirection += 360f;
                }

                closeEnough = Mathf.Abs(curXDirection - _controllerLeftHand.transform.rotation.eulerAngles.y) <= _smartDecoupleThreshold;
                closeEnough = closeEnough && (Mathf.Abs(curXDirection - _controllerRightHand.transform.rotation.eulerAngles.y) <= _smartDecoupleThreshold);

                // If the controllers and the HMD are pointing the same direction (within the threshold) reset the direction
                if (closeEnough)
                {
                    _initialGaze = hmdPosition();
                }
            }

            return _initialGaze;
        }

        private Vector3 calculateControllerRotationDirection(Vector3 calculatedControllerDirection)
        {
            return (Vector3.Angle(_previousDirection, calculatedControllerDirection) <= 90f ? calculatedControllerDirection : _previousDirection);
        }

        private void setDeltaTransformData()
        {
            for (int i = 0; i < _trackedObjects.Count; i++)
            {
                var trackedObj = _trackedObjects[i];

                // Get delta postions and rotations
                _previousYPositions[trackedObj] = trackedObj.transform.localPosition.y;
            }
        }

        private void movePlayArea(Vector3 moveDirection, float moveSpeed)
        {
            var movement = (moveDirection * moveSpeed) * Time.fixedDeltaTime;
            if (_playArea != null)
            {
                var finalPosition = new Vector3(movement.x + _playArea.position.x, _playArea.position.y, movement.z + _playArea.position.z);
                _playArea.position = finalPosition;
            }
        }

        private void OnTriggerPressedOrReleased(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            if ((fromSource == SteamVR_Input_Sources.LeftHand && !_useLeftHandActivation) ||
                (fromSource == SteamVR_Input_Sources.RightHand && !_useRightHandActivation))
            {
                return;
            }

            if (newState)
            {
                _engagedController = fromSource;
                _movementEngaged = true;
            }
            else
            {
                // If the button is released clear all the lists.
                for (int i = 0; i < _trackedObjects.Count; i++)
                {
                    _movementList[_trackedObjects[i]].Clear();
                }

                _initialGaze = Vector3.zero;

                _movementEngaged = false;
                _engagedController = SteamVR_Input_Sources.Any;
            }
        }

        private Quaternion determineAverageControllerRotation()
        {
            // Build the average rotation of the controller(s)
            var newRotation = Quaternion.identity;

            // Both controllers are present
            if (_controllerLeftHand != null && _controllerRightHand != null)
            {
                newRotation = averageRotation(_controllerLeftHand.transform.rotation, _controllerRightHand.transform.rotation);
            }
            // Left controller only
            else if (_controllerLeftHand != null && _controllerRightHand == null)
            {
                newRotation = _controllerLeftHand.transform.rotation;
            }
            // Right controller only
            else if (_controllerRightHand != null && _controllerLeftHand == null)
            {
                newRotation = _controllerRightHand.transform.rotation;
            }

            return newRotation;
        }

        private Quaternion averageRotation(Quaternion rot1, Quaternion rot2)
        {
            return Quaternion.Slerp(rot1, rot2, 0.5f);
        }
    }
}