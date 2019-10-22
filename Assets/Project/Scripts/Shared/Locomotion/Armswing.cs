using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Armswing : MonoBehaviour
{

    /***** CLASS VARIABLES *****/

    /** Inspector Variables **/

    // ReadMe Location
    public string GithubProjectAndDocs = "https://github.com/ElectricNightOwl/ArmSwinger";
    public bool generalScaleWorldUnitsToCameraRigScale = false;
    public bool generalAutoAdjustFixedTimestep = true;

    public bool armSwingNavigation = true;
    public AnimationCurve armSwingControllerToMovementCurve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
    private float _armSwingControllerSpeedForMaxSpeed = 3f;
    private float _armSwingMaxSpeed = 8;
    private float _armSwingBothControllersCoefficient = 1.0f;
    [SerializeField]
    private float _armSwingSingleControllerCoefficient = .7f;

    // Controller Smoothing Settings
    public bool controllerSmoothing = true;
      public int controllerSmoothingCacheSize = 3;

    // Inertia Settings
    private bool _movingInertia = true;
    public float movingInertiaTimeToStopAtMaxSpeed = .5f;
    private bool _stoppingInertia = true;
    public float stoppingInertiaTimeToStopAtMaxSpeed = .35f;

    public LayerMask raycastGroundLayerMask = -1;
    private float _raycastMaxLength = 100f;
    private int _raycastAverageHeightCacheSize = 3;
    public bool raycastOnlyHeightAdjustWhileArmSwinging = false;

    private bool _preventWallClip = true;
    public LayerMask preventWallClipLayerMask = -1;
    public PreventionMode preventWallClipMode = PreventionMode.PushBack;
    private float _preventWallClipHeadsetColliderRadius = .11f;
    private float _preventWallClipMinAngleToTrigger = 20f;

    public bool preventClimbing = true;
    private float _preventClimbingMaxAnglePlayerCanClimb = 45f;
    public bool preventFalling = true;
    private float _preventFallingMaxAnglePlayerCanFall = 60f;
    public bool preventWallWalking = true;
    private float _instantHeightMaxChange = .2f;
    public PreventionMode instantHeightClimbPreventionMode = PreventionMode.PushBack;
    public PreventionMode instantHeightFallPreventionMode = PreventionMode.Rewind;

    public int checksNumClimbFallChecksOOBBeforeRewind = 5;
    public int checksNumWallWalkChecksOOBBeforeRewind = 15;

    public bool pushBackOverride = true;
    public float pushBackOverrideRefillPerSec = 30;
    public float pushBackOverrideMaxTokens = 90;

    private float _rewindMinDistanceChangeToSavePosition = .05f;
    public bool rewindDontSaveUnsafeClimbFallPositions = true;
    public bool rewindDontSaveUnsafeWallWalkPositions = true;
    public int rewindNumSavedPositionsToStore = 28;
    public int rewindNumSavedPositionsToRewind = 7;
    public float rewindFadeOutSec = .15f;
    public float rewindFadeInSec = .35f;

    public enum PreventionReason { CLIMBING, FALLING, INSTANT_CLIMBING, INSTANT_FALLING, OHAWAS, HEADSET, WALLWALK, MANUAL, NO_GROUND, NONE };
    public enum PreventionMode { Rewind, PushBack };
    private enum PreventionCheckType { CLIMBFALL, WALLWALK };

    public enum ControllerSmoothingMode { Lowest, Average, AverageMinusHighest };

    // Informational public bools
    [HideInInspector]
    public bool outOfBounds = false;
    [HideInInspector]
    public bool rewindInProgress = false;
    [HideInInspector]
    public bool armSwinging = false;


    private bool _armSwingingPaused = false;
    [SerializeField]
    [Tooltip("Preventions Paused\n\nPauses all prevention methods (Climbing, Falling, Instant, Wall Clip, etc) while true.\n\n(Default: false)")]
    private bool _preventionsPaused = false;
    [SerializeField]
    [Tooltip("Angle Preventions Paused\n\nPauses all angle-based prevention methods (Climbing, Falling, Instant) while true.\n\n(Default: false)")]
    private bool _anglePreventionsPaused = false;
    [SerializeField]
    [Tooltip("Wall Clip Prevention Paused\n\nPauses wall clip prevention while true.\n\n(Default: false)")]
    private bool _wallClipPreventionPaused = false;
    [SerializeField]
    [Tooltip("Play Area Height Adjustment Paused\n\nPauses play area height adjustment unconditionally.  When this is changed from true to false, the play area will immediately be adjusted to the ground.\n\n(Default: false)")]
    private bool _playAreaHeightAdjustmentPaused = false;

    // Controller positions
    private GameObject m_LeftController;
    private GameObject m_RightController;
    private Vector3 leftControllerLocalPosition;
    private Vector3 rightControllerLocalPosition;
    private Vector3 leftControllerPreviousLocalPosition;
    private Vector3 rightControllerPreviousLocalPosition;

    // Headset/Camera Rig saved position history
    private LinkedList<Vector3> previousHeadsetRewindPositions = new LinkedList<Vector3>();
    private Vector3 lastHeadsetRewindPositionSaved = new Vector3(0, 0, 0);
    private LinkedList<Vector3> previousCameraRigRewindPositions = new LinkedList<Vector3>();
    private Vector3 lastCameraRigRewindPositionSaved = new Vector3(0, 0, 0);
    private Vector3 previousAngleCheckHeadsetPosition;

    // Pushback positions
    private int previousPushbackPositionSize = 5;
    private LinkedList<Vector3> previousCameraRigPushBackPositions = new LinkedList<Vector3>();
    private LinkedList<Vector3> previousHeadsetPushBackPositions = new LinkedList<Vector3>();

    // RaycastHit histories
    private List<RaycastHit> headsetCenterRaycastHitHistoryPrevention = new List<RaycastHit>(); // History of headset center RaycastHits used for prevention checks
    private List<RaycastHit> headsetCenterRaycastHitHistoryHeight = new List<RaycastHit>(); // History of headset center RaycastHits used for height adjustments each frame
    private RaycastHit lastRaycastHitWhileArmSwinging; // The last every-frame headset center RaycastHit that was seen while the player was arm swinging

    // Prevention Reason histories
    private Queue<PreventionReason> climbFallPreventionReasonHistory = new Queue<PreventionReason>();
    private Queue<PreventionReason> wallWalkPreventionReasonHistory = new Queue<PreventionReason>();
    private PreventionReason _currentPreventionReason = PreventionReason.NONE;

    // Controller Movement Result History
    private LinkedList<float> controllerMovementResultHistory = new LinkedList<float>(); // The controller movement results after the Swing Mode calculations but before inertia and 1/2-hand coefficients

    // Saved angles
    private float latestCenterChangeAngle;
    private float latestSideChangeAngle;

    // Saved movement
    private float latestArtificialMovement;
    private Quaternion latestArtificialRotation;
    private float previousTimeDeltaTime = 0f;
    private Vector3 previousAngleCheckCameraRigPosition;

    // Inertia curves
    // WARNING: must be linear for now
    private AnimationCurve movingInertiaCurve = new AnimationCurve(new Keyframe(0, 1, -1, -1), new Keyframe(1, 0, -1, -1));
    private AnimationCurve stoppingInertiaCurve = new AnimationCurve(new Keyframe(0, 1, -1, -1), new Keyframe(1, 0, -1, -1));


    //// Controller buttons ////
    public SteamVR_Action_Boolean m_ArmSwingButton = null;
    private bool leftButtonPressed = false;
    private bool rightButtonPressed = false;

    //// Controllers ////
    public GameObject leftControllerGameObject;
    public GameObject rightControllerGameObject;

    // Wall Clip tracking
    [HideInInspector]
    public bool wallClipThisFrame = false;
    [HideInInspector]
    public bool rewindThisFrame = false;

    // Push back override
    private float pushBackOverrideValue;
    private bool pushBackOverrideActive = false;

    // One Button Same Controller Exclusive mode only
    private GameObject activeSwingController = null;

    // Prevent Wall Clip's HeadsetCollider script
    private HeadsetCollider headsetCollider;

    // GameObjects
    public GameObject headsetGameObject;
    public GameObject cameraRigGameObject;

    // Camera Rig scaling
    private float cameraRigScaleModifier = 1.0f;

    /****** INITIALIZATION ******/
    void Awake()
    {
        m_ArmSwingButton[SteamVR_Input_Sources.LeftHand].onStateUp += LeftRelease;
        m_ArmSwingButton[SteamVR_Input_Sources.LeftHand].onStateDown += LeftPushed;
        m_ArmSwingButton[SteamVR_Input_Sources.RightHand].onStateUp += RightRelease;
        m_ArmSwingButton[SteamVR_Input_Sources.RightHand].onStateDown += RightPushed;

        // Setup wall clipping on the headset gameobject, if enabled
        if (preventWallClip)
        {
            setupHeadsetCollider();
        }

        // There is some variation in the degree measurement per-frame in Unity.  As such, we increase the maximum allowed angle
        // very slightly to provide consistent results when climbing/falling a surface that is exactly the user-set value.
        // Yes, this is lame.  Sorry.
        preventClimbingMaxAnglePlayerCanClimb += .005f;
        preventFallingMaxAnglePlayerCanFall += .005f;

        // Seed the initial previousLocalPositions
        leftControllerPreviousLocalPosition = leftControllerGameObject.transform.localPosition;
        rightControllerPreviousLocalPosition = rightControllerGameObject.transform.localPosition;

        seedSavedPositions();

        // Seed the initial saved position
        saveRewindPosition(true);
        previousAngleCheckCameraRigPosition = cameraRigGameObject.transform.position;
        lastRaycastHitWhileArmSwinging = raycast(headsetGameObject.transform.position, Vector3.down, raycastMaxLength, raycastGroundLayerMask);

        // Pre-seed previousTimeDeltaTime
        previousTimeDeltaTime = 1f / 90f;

        // Refill Push Back Override completely
        pushBackOverrideValue = pushBackOverrideMaxTokens;

    }

    /***** FIXED UPDATE *****/
    void FixedUpdate()
    {

        // Set scale as necessary (defaults to 1.0)
        // Doing this in Update() allows the Camera Rig to be scaled during runtime but keep the same ArmSwinger feel
        if (generalScaleWorldUnitsToCameraRigScale)
        {
            cameraRigScaleModifier = this.transform.localScale.x;
        }

        // Push Back Override
        if (pushBackOverride && !rewindInProgress)
        {
            incrementPushBackOverride();
        }

        // reset this frame counters
        rewindThisFrame = false;
        wallClipThisFrame = false;

        // Check for wall clipping
        if (preventWallClip && headsetCollider.inGeometry)
        {
            triggerRewind(PreventionReason.HEADSET);
        }

        // Save the current controller positions for our use
        leftControllerLocalPosition = leftControllerGameObject.transform.localPosition;
        rightControllerLocalPosition = rightControllerGameObject.transform.localPosition;

        // Variable motion based on controller movement
        if (armSwingNavigation &&
            !outOfBounds &&
            !wallClipThisFrame &&
            !rewindThisFrame &&
            !armSwingingPaused &&
            !rewindInProgress)
        {
            transform.position += variableArmSwingMotion();
        }

        // Save the current controller positions for next frame
        leftControllerPreviousLocalPosition = leftControllerGameObject.transform.localPosition;
        rightControllerPreviousLocalPosition = rightControllerGameObject.transform.localPosition;

        // Only copy safe spots for push backs
        if (!outOfBounds && !wallClipThisFrame && !rewindThisFrame)
        {
            if ((previousCameraRigPushBackPositions.Last.Value != cameraRigGameObject.transform.position) ||
                (previousHeadsetPushBackPositions.Last.Value != headsetGameObject.transform.position))
            {

                savePosition(previousCameraRigPushBackPositions, cameraRigGameObject.transform.position, previousPushbackPositionSize);
                savePosition(previousHeadsetPushBackPositions, headsetGameObject.transform.position, previousPushbackPositionSize);
            }
        }

        // Adjust the camera rig height, and prevent climbing/falling as configured
        if (!wallClipThisFrame && !rewindThisFrame && !outOfBounds)
        {
            adjustCameraRig();
        }

        // Save this Time.deltaTime for next frame (inertia simulation)
        previousTimeDeltaTime = Time.deltaTime;
    }

    private void LeftRelease(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        leftButtonPressed = false;
    }

    private void LeftPushed(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        leftButtonPressed = true;
    }

    private void RightRelease(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        rightButtonPressed = false;
    }

    private void RightPushed(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        rightButtonPressed = true;
    }



    /***** CORE FUNCTIONS *****/
    // Variable Arm Swing locomotion
    Vector3 variableArmSwingMotion()
    {

        // Initialize movement variables
        float movementAmount = 0f;
        Quaternion movementRotation = Quaternion.identity;
        bool movedThisFrame = false;

        // if the player isn't ArmSwinging (or at least wasn't as of the last frame), clear the movement amount history
        if (!armSwinging)
        {
            controllerMovementResultHistory.Clear();
        }

        if (swingOneButtonSameController(ref movementAmount, ref movementRotation))
        {
            // If raycastOnlyHeightAdjustWhileArmSwinging is enabled, check to see if the Y distance between the previous arm swinging position and
            // the current ArmSwinging position are higher than the instant height change max.  This ensures that players who have 
            // raycastOnlyHeightAdjustWhileArmSwinging enabled are not instantly teleported to the terrain when they start arm swinging.
            if (!armSwinging && raycastOnlyHeightAdjustWhileArmSwinging)
            {
                armSwinging = true;

                RaycastHit startSwingingRaycastHit = raycast(headsetGameObject.transform.position, Vector3.down, raycastMaxLength, raycastGroundLayerMask, out bool didStartSwingingRayHit);

                if (didStartSwingingRayHit)
                {
                    ohawasInstantHeightChangeCheck(startSwingingRaycastHit.point.y, lastRaycastHitWhileArmSwinging.point.y);
                    // If we need to rewind, don't arm swing this frame
                    if (currentPreventionReason != PreventionReason.NONE)
                    {
                        return Vector3.zero;
                    }
                }
            }

            armSwinging = true;

            latestArtificialMovement = movementAmount;
            latestArtificialRotation = movementRotation;

            // Move forward in the X and Z axis only (no flying!)
            return getForwardXZ(movementAmount, movementRotation);

        }
        else
        {
            armSwinging = false;

            return Vector3.zero;
        }
    }

    bool swingOneButtonSameController(ref float movement, ref Quaternion rotation)
    {
        if (leftButtonPressed && rightButtonPressed)
        {
            // The rotation is the average of the two controllers
            rotation = determineAverageControllerRotation();

            // Find the change in controller position since last Update()
            float leftControllerChange = Vector3.Distance(leftControllerPreviousLocalPosition, leftControllerLocalPosition);
            float rightControllerChange = Vector3.Distance(rightControllerPreviousLocalPosition, rightControllerLocalPosition);

            // Calculate what camera rig movement the change should be converted to
            float leftMovement = calculateMovement(armSwingControllerToMovementCurve, leftControllerChange, armSwingControllerSpeedForMaxSpeed, armSwingMaxSpeed);
            float rightMovement = calculateMovement(armSwingControllerToMovementCurve, rightControllerChange, armSwingControllerSpeedForMaxSpeed, armSwingMaxSpeed);

            // Both controllers are in use, so controller movement is the average of the two controllers' change times the both controller coefficient
            float controllerMovement = (leftMovement + rightMovement) / 2 * armSwingBothControllersCoefficient;

            // If movingInertia is enabled, the higher of inertia or controller movement is used
            if (movingInertia)
            {
                movement = movingInertiaOrControllerMovement(controllerMovement);
            }
            else
            {
                movement = controllerMovement;
            }

            return true;
        }
        else if (leftButtonPressed)
        {
            // The rotation is the rotation of the left controller
            rotation = leftControllerGameObject.transform.rotation;

            // Find the change in controller position since last Update()
            float leftControllerChange = Vector3.Distance(leftControllerPreviousLocalPosition, leftControllerLocalPosition);

            // Calculate what camera rig movement the change should be converted to
            float leftMovement = calculateMovement(armSwingControllerToMovementCurve, leftControllerChange, armSwingControllerSpeedForMaxSpeed, armSwingMaxSpeed);

            // controller movement is the change of the left controller times the single controller coefficient
            float controllerMovement = leftMovement * armSwingSingleControllerCoefficient;

            // If movingInertia is enabled, the higher of inertia or controller movement is used
            if (movingInertia)
            {
                movement = movingInertiaOrControllerMovement(controllerMovement);
            }
            else
            {
                movement = controllerMovement;
            }

            return true;
        }
        else if (rightButtonPressed)
        {
            // The rotation is the rotation of the right controller
            rotation = rightControllerGameObject.transform.rotation;

            // Find the change in controller position since last Update()
            float rightControllerChange = Vector3.Distance(rightControllerPreviousLocalPosition, rightControllerLocalPosition);

            // Calculate what camera rig movement the change should be converted to
            float rightMovement = calculateMovement(armSwingControllerToMovementCurve, rightControllerChange, armSwingControllerSpeedForMaxSpeed, armSwingMaxSpeed);

            // controller movement is the change of the right controller times the single controller coefficient
            float controllerMovement = rightMovement * armSwingSingleControllerCoefficient;

            // If movingInertia is enabled, the higher of inertia or controller movement is used
            if (movingInertia)
            {
                movement = movingInertiaOrControllerMovement(controllerMovement);
            }
            else
            {
                movement = controllerMovement;
            }

            return true;
        }
        // If stopping inertia is enabled
        else if (stoppingInertia && latestArtificialMovement != 0)
        {

            // The rotation is the cached one
            rotation = latestArtificialRotation;
            // The stopping movement is calculated using a curve, the latest movement, last frame's deltaTime, max speed, and the stop time for max speed
            movement = inertiaMovementChange(stoppingInertiaCurve, latestArtificialMovement, previousTimeDeltaTime, armSwingMaxSpeed, stoppingInertiaTimeToStopAtMaxSpeed);

            return true;
        }
        else
        {
            return false;
        }
    }

    float movingInertiaOrControllerMovement(float movement)
    {

        if (controllerSmoothing)
        {
            // Save the movement amount for moving inertia calculations
            saveFloatToLinkedList(controllerMovementResultHistory, movement, controllerSmoothingCacheSize);

            movement = smoothedControllerMovement(controllerMovementResultHistory);

        }

        float inertiaMovement = inertiaMovementChange(movingInertiaCurve, latestArtificialMovement, previousTimeDeltaTime, armSwingMaxSpeed, movingInertiaTimeToStopAtMaxSpeed);

        if (inertiaMovement >= movement)
        {
            return inertiaMovement;
        }
        else
        {
            return movement;
        }
    }

    // Using a linear curve, the movement last frame, the max speed we can go, and the time it should take to stop at that max speed,
    // compute the amount of movement that should happen THIS frame.  Note that timeToStopAtMaxSpeed is only if the player is going armSwingMaxSpeed.  If the
    // player is going LESS than armSwingMaxSpeed, the time to stop will be a percentage of timeToStopAtMaxSpeed.
    //
    // I tried implemeting this with custom curves, but ran into an issue where I couldn't determine where in the curve to start given the player's 
    // current speed.  For now, I'm making it linear-only, which works fine.  Would be amazing to make it work with arbitrary curves in the future.
    //
    // Also, I guarantee this can be done better.  I should have paid more attention in math.  Sorry, Mrs. Powell.

    // TODO: Make work with custom curves instead of linear only
    static float inertiaMovementChange(AnimationCurve curve, float latestMovement, float latestTimeDeltaTime, float maxSpeed, float timeToStopAtMaxSpeed)
    {

        // Max speed in Movement Per Frame
        float maxSpeedInMPF = maxSpeed * Time.deltaTime;

        // Frames per second
        float fps = 1 / Time.deltaTime;

        // The percentage through the curve the last movement was (based on the previous frame's Time.deltaTime)
        float percentThroughCurve = 1 - (latestMovement / (maxSpeed * latestTimeDeltaTime));

        // The percentage change in speed that needs to happen each frame based on the current frame rate
        float percentChangeEachFrame = 1 / (timeToStopAtMaxSpeed * fps);

        // Calculate the new percentage through the curve by adding the percent change each frame to the last percent
        float newPercentThroughCurve = percentThroughCurve + percentChangeEachFrame;

        // Evaluate the curve at the new percentage to determine how what percentage of armSwingMaxSpeed we should be going this frame
        float curveEval = curve.Evaluate(newPercentThroughCurve);

        // Set the movement value based on the evaluated curve, multiplied by the max Speed in Movement Per Frame
        float inertiaMovement = curveEval * maxSpeedInMPF;

        if (inertiaMovement <= 0f)
        {
            inertiaMovement = 0f;
        }

        return inertiaMovement;

    }

    // adjustCameraRig has two main functions, Prevent Climbing/Falling/Wall Walk/Wall Clip and adjustment of the play area height.
    // A raycast from the HMD downwards to the terrain is used to determine how far away the headset is from the ground.
    // That raycast is then compared to the previous raycast to determine the angle of the ground and if a prevention method
    // should be tripped.  Those decisions are collected, and when numChecksOOBBeforeRewind frames are in agreement, a rewind is
    // initiated.
    //
    // Second, the same raycast information is used to adjust the play area's Y coordinates so that the play space follows the
    // contours of the terrain as the player moves around.
    void adjustCameraRig()
    {

        if (outOfBounds)
        {
            return;
        }

        //// Center Headset Raycast ////
        bool didHeadsetCenterRayHit;
        RaycastHit headsetCenterRaycastHit = raycast(headsetGameObject.transform.position, Vector3.down, raycastMaxLength, raycastGroundLayerMask, out didHeadsetCenterRayHit);

        // Save the raycast hit for averaging movement (unconditional)
        saveRaycastHit(headsetCenterRaycastHitHistoryHeight, headsetCenterRaycastHit, raycastAverageHeightCacheSize);

        // If there are not at least 2 RaycastHits in the history, add this one again
        while (headsetCenterRaycastHitHistoryHeight.Count < 2)
        {
            saveRaycastHit(headsetCenterRaycastHitHistoryHeight, headsetCenterRaycastHit, raycastAverageHeightCacheSize);
        }

        // Only if this Ray hits the ground
        if (didHeadsetCenterRayHit)
        {

            if (armSwinging)
            {
                lastRaycastHitWhileArmSwinging = headsetCenterRaycastHit;
            }

            //// Prevent Climbing/Falling/Wall Walking ////
            bool checkAnglesThisFrame = false;
            if ((preventClimbing || preventFalling || preventWallWalking) && !outOfBounds)
            {

                checkAnglesThisFrame = isDistanceFarEnough(previousAngleCheckHeadsetPosition, headsetGameObject.transform.position, rewindMinDistanceChangeToSavePosition);

                // Check for too much travel either up or down (climb a cliff in one bound, or fell off a cliff)
                // This is fired every frame
                RaycastHit previousHeadsetCenterRaycastHit = headsetCenterRaycastHitHistoryHeight[headsetCenterRaycastHitHistoryHeight.Count - 2];
                instantHeightChangeCheck(headsetCenterRaycastHit.point.y, previousHeadsetCenterRaycastHit.point.y);

                // Only fire angle-based checks if the player has travelled far enough since the last check
                // Uses the headset position to determine this
                // Only fires if we're not already out of bounds due to instantHeightChangeCheck
                if (checkAnglesThisFrame && !outOfBounds)
                {

                    while (headsetCenterRaycastHitHistoryPrevention.Count < 2)
                    {
                        saveRaycastHit(headsetCenterRaycastHitHistoryPrevention, headsetCenterRaycastHit, 2);
                    }

                    saveRaycastHit(headsetCenterRaycastHitHistoryPrevention, headsetCenterRaycastHit, 2);

                    // Check for Climbing/Falling too steeply
                    centerPreventionCheck(headsetCenterRaycastHit, headsetCenterRaycastHitHistoryPrevention[headsetCenterRaycastHitHistoryPrevention.Count - 2]);

                    // If Prevent Wall Walking is enabled, generate a second ray cast just to the side of the headset middle one.
                    if (preventWallWalking && !outOfBounds)
                    {

                        // First, get the correct position of the side ray in 2D space
                        Vector3 headsetSideRayOriginXZ = new Vector3();
                        if (headsetCenterRaycastHitHistoryPrevention.Count >= 2)
                        {
                            headsetSideRayOriginXZ = determineSideRayOriginXZ(headsetCenterRaycastHitHistoryPrevention[1], headsetCenterRaycastHitHistoryPrevention[0], .001f);
                        }
                        else
                        {
                            headsetSideRayOriginXZ = headsetCenterRaycastHitHistoryPrevention[0].point;
                        }

                        // Raise the result to match the height of the headset
                        Vector3 headsetSideRayOrigin = headsetSideRayOriginXZ + new Vector3(0f, headsetGameObject.transform.position.y, 0f);

                        //// Left Headset Raycast ////
                        // Shoot the Ray from the headset position, left (local) a smidge, and .1m above (local) the head, straight down (world)
                        bool didHeadsetLeftRayHit;
                        RaycastHit headsetSideRaycastHit = raycast(headsetSideRayOrigin, Vector3.down, raycastMaxLength, raycastGroundLayerMask, out didHeadsetLeftRayHit);
                        sidePreventionCheck(headsetCenterRaycastHit, headsetSideRaycastHit);
                    }

                    // Save this position as the last one prevention checked
                    previousAngleCheckHeadsetPosition = headsetGameObject.transform.position;
                    previousAngleCheckCameraRigPosition = cameraRigGameObject.transform.position;

                }
            }

            if (checkAnglesThisFrame && !outOfBounds && !wallClipThisFrame && !rewindThisFrame)
            {
                // Save the current non-out-of-bounds headset and play area position
                saveRewindPosition();
            }

            if (!outOfBounds && !wallClipThisFrame && currentPreventionReason == PreventionReason.NONE && !playAreaHeightAdjustmentPaused)
            {
                if ((raycastOnlyHeightAdjustWhileArmSwinging && armSwinging) || (!raycastOnlyHeightAdjustWhileArmSwinging))
                {
                    if (headsetCenterRaycastHitHistoryHeight.Count > 0)
                    {
                        // Move the camera to adjust to the ground
                        cameraRigGameObject.transform.position = new Vector3(
                            cameraRigGameObject.transform.position.x,
                            averageRaycastHitY(headsetCenterRaycastHitHistoryHeight),
                            cameraRigGameObject.transform.position.z);
                    }
                }
            }
        }
        else
        {
            //triggerRewind(PreventionReason.NO_GROUND);
        }
    }

    PreventionReason instantHeightChangeReason(float thisYValue, float lastYValue)
    {

        float heightDifference = thisYValue - lastYValue;

        if (preventClimbing && heightDifference >= instantHeightMaxChange)
        {
            return PreventionReason.INSTANT_CLIMBING;
        }
        else if (preventFalling && heightDifference <= -instantHeightMaxChange)
        {
            return PreventionReason.INSTANT_FALLING;
        }
        else
        {
            return PreventionReason.NONE;
        }
    }

    void instantHeightChangeCheck(float thisYValue, float lastYValue)
    {

        if (preventionsPaused || anglePreventionsPaused)
        {
            return;
        }

        // We allow players to climb/descend stairs instantly (across one frame) as long as the stair is shorter than instantHeightMaxChange
        // If the current raycast and the previous raycast indicate a height difference larger than instantHeightMaxChange,
        // we know for sure that the player needs to be rewound.

        // This also prevents players from taking a long fall over a single frame (since multiple frames in a row need to agree to rewind normally)

        // Finally, this also affects raycastOnlyHeightAdjustWhileArmSwinging.  If the player moves around physically, and then starts Arm Swinging again,
        // we'll instantly check to see if they've changed more than instantHeightMaxChange.  If they have, we'll do a quick rewind to ensure
        // they start in a comfortable position

        PreventionReason preventionReason = instantHeightChangeReason(thisYValue, lastYValue);

        if (preventionReason != PreventionReason.NONE)
        {
            triggerRewind(preventionReason);
        }
    }

    // Instant Height Change Check for when raycastOnlyHeightAdjustWhileArmSwinging (OHAWAS) is enabled, and the player starts ArmSwinging
    void ohawasInstantHeightChangeCheck(float thisYValue, float lastYValue)
    {
        if (instantHeightChangeReason(thisYValue, lastYValue) != PreventionReason.NONE)
        {
            triggerRewind(PreventionReason.OHAWAS);
        }
    }

    void centerPreventionCheck(RaycastHit thisRaycastHit, RaycastHit lastRaycastHit)
    {

        PreventionReason thisOOBReason = getOutOfBoundsReason(PreventionCheckType.CLIMBFALL, thisRaycastHit, lastRaycastHit);
        saveReason(climbFallPreventionReasonHistory, thisOOBReason, checksNumClimbFallChecksOOBBeforeRewind);

        if (thisOOBReason != PreventionReason.NONE)
        {
        }

        if (thisOOBReason != PreventionReason.NONE && !outOfBounds && climbFallPreventionReasonHistory.Count == checksNumClimbFallChecksOOBBeforeRewind)
        {
            bool allReasonsAgree = true;

            Queue<PreventionReason> checkQueue = climbFallPreventionReasonHistory;
            while (allReasonsAgree && checkQueue.Count > 0)
            {
                if (checkQueue.Dequeue() != thisOOBReason)
                {
                    allReasonsAgree = false;
                }
            }

            if (allReasonsAgree)
            {
                triggerRewind(thisOOBReason);
            }
        }
    }

    void sidePreventionCheck(RaycastHit centerRaycastHit, RaycastHit sideRaycastHit)
    {
        PreventionReason thisOOBReason = getOutOfBoundsReason(PreventionCheckType.WALLWALK, centerRaycastHit, sideRaycastHit);

        // We save 2x the wall walk reasons required for a rewind
        saveReason(wallWalkPreventionReasonHistory, thisOOBReason, checksNumWallWalkChecksOOBBeforeRewind * 2);

        // Some logic explanation is needed here.  If a player is wall walking up and at a constant angle, using a cache of size numWallWalkChecksOOBBeforeRewind
        // works fine.  However, if the player periodically angles themselves so that a wall walk is not detected, all the reasons in the cache will NEVER agree.
        // To prevent this exploit, we do a form of sampling.  The wall walk history is 2*numWallWalkChecksOOBBeforeRewind, but only numWallWalkChecksOOBBeforeRewind
        // checks need to agree to trigger a rewind.  
        //
        // TODO: Make the 2x multiplier for wall walk check cache adjustable

        if (thisOOBReason != PreventionReason.NONE && !outOfBounds && wallWalkPreventionReasonHistory.Count >= checksNumWallWalkChecksOOBBeforeRewind)
        {
            int wallWalkCount = 0;

            // Examine the entire cache and count the number of wall walks
            Queue<PreventionReason> checkQueue = new Queue<PreventionReason>(wallWalkPreventionReasonHistory);

            while (wallWalkCount < checksNumWallWalkChecksOOBBeforeRewind && checkQueue.Count > 0)
            {
                if (checkQueue.Dequeue() == PreventionReason.WALLWALK)
                {
                    wallWalkCount++;
                }
            }

            // If the number of wall walks is >= numWallWalkChecksOOBBeforeRewind, trigger a rewind
            if (wallWalkCount >= checksNumWallWalkChecksOOBBeforeRewind)
            {
                triggerRewind(thisOOBReason);
            }
        }
    }

    PreventionReason getOutOfBoundsReason(PreventionCheckType checkType, RaycastHit thisHit, RaycastHit lastHit)
    {

        if (currentPreventionReason == PreventionReason.HEADSET)
        {
            return PreventionReason.HEADSET;
        }
        else if (preventionsPaused || anglePreventionsPaused)
        {
            return PreventionReason.NONE;
        }

        //  |\
        //  | \
        // A|  \ C
        //  |   \
        //  |____\
        //     B
        float a = thisHit.point.y - lastHit.point.y;
        float b = Vector2.Distance(new Vector2(thisHit.point.x, thisHit.point.z), new Vector2(lastHit.point.x, lastHit.point.z));

        float angleOfChange = Mathf.Atan(a / b) * Mathf.Rad2Deg;

        // Climbing/falling checking
        if (checkType == PreventionCheckType.CLIMBFALL)
        {

            latestCenterChangeAngle = angleOfChange;

            // Prevent Climbing
            if (preventClimbing && angleOfChange > preventClimbingMaxAnglePlayerCanClimb && currentPreventionReason != PreventionReason.FALLING)
            {
                return PreventionReason.CLIMBING;
            }
            // Prevent Falling
            else if (preventFalling && angleOfChange < -preventFallingMaxAnglePlayerCanFall && currentPreventionReason != PreventionReason.CLIMBING)
            {
                return PreventionReason.FALLING;
            }
            else
            {
                return PreventionReason.NONE;
            }
        }
        // Prevent Wall Walking
        else if (checkType == PreventionCheckType.WALLWALK)
        {

            latestSideChangeAngle = angleOfChange;

            float cameraRigPositionDifferenceY = cameraRigGameObject.transform.position.y - previousAngleCheckCameraRigPosition.y;

            // If the camera rig is moving up, check against preventClimbingMaxAnglePlayerCanClimb
            // If the camera rig is moving down, check against preventFallingMaxAnglePlayerCanFall
            if ((cameraRigPositionDifferenceY >= .0001f && Mathf.Abs(angleOfChange) > preventClimbingMaxAnglePlayerCanClimb) ||
                (cameraRigPositionDifferenceY <= -.0001f && Mathf.Abs(angleOfChange) > preventFallingMaxAnglePlayerCanFall))
            {
                if (Mathf.Abs(angleOfChange) > preventClimbingMaxAnglePlayerCanClimb)
                {
                    return PreventionReason.WALLWALK;
                }
                else
                {
                    return PreventionReason.NONE;
                }
            }
        }

        return PreventionReason.NONE;
    }

    public void triggerRewind(PreventionReason reason = PreventionReason.MANUAL)
    {

        currentPreventionReason = reason;

        if (reason == PreventionReason.HEADSET)
        {
            wallClipThisFrame = true;
        }

        //Debug.Log(Time.frameCount + "|ArmSwinger.triggerRewind():: Rewind triggered due to " + reason);

        if (!outOfBounds)
        {
            // Special handling for raycastOnlyHeightAdjustWhileArmSwinging (OHAWAS) events where the player walks into geometry and then starts arm swinging.
            if (reason == PreventionReason.OHAWAS)
            {
                outOfBounds = true;
                fadeOut();
                Invoke("ohawasCameraRigAdjust", rewindFadeOutSec);
            }
            // Everything else
            else
            {
                outOfBounds = true;

                // If the prevention mode is REWIND and a rewind isn't already pending - fade out, rewind, fade back in
                if (currentPreventionMode == PreventionMode.Rewind && !rewindInProgress)
                {
                    rewindInProgress = true;
                    fadeOut();
                    Invoke("rewindPositionModeRewind", rewindFadeOutSec);
                    Invoke("fadeIn", rewindFadeOutSec);
                }
                // Otherwise the mode is PushBack, so we instantly push back
                else
                {
                    rewindPosition(PreventionMode.PushBack);
                    if (pushBackOverride)
                    {
                        decrementPushBackOverride();
                    }
                }
            }
        }
    }

    // Helper function for the Invoke() in triggerRewind, since Invoke doesn't support any parameters in called functions
    void rewindPositionModeRewind()
    {
        rewindPosition(PreventionMode.Rewind);
    }

    void rewindPosition(PreventionMode preventionMode)
    {

        // Let other features know that a rewind occured this frame
        rewindThisFrame = true;

        // Reset all caches
        resetReasonHistory();
        resetRaycastHitHistory();
        latestArtificialMovement = 0f;
        latestArtificialRotation = Quaternion.identity;

        // The positions we'll be rewinding to
        Vector3 cameraRigPreviousPositionToRewindTo = new Vector3();
        Vector3 headsetPreviousPositionToRewindTo = new Vector3();

        // Determine what previous positions we need to rewind to
        determinePreviousPositionToRewindTo(ref cameraRigPreviousPositionToRewindTo, ref headsetPreviousPositionToRewindTo, preventionMode);

        Vector3 newCameraRigPosition = calculateCameraRigRewindPosition(cameraRigPreviousPositionToRewindTo, headsetPreviousPositionToRewindTo, cameraRigGameObject.transform.position, headsetGameObject.transform.position, preventionMode);

        cameraRigGameObject.transform.position = newCameraRigPosition;

        previousAngleCheckHeadsetPosition = headsetGameObject.transform.position;

        // Seed caches with the new, safe position
        seedRaycastHitHistory();
        seedSavedPositions();

        //resetOOB();
        if (preventionMode == PreventionMode.Rewind)
        {
            Invoke("resetOOB", rewindFadeInSec);
            rewindInProgress = false;
        }
        else if (preventionMode == PreventionMode.PushBack)
        {
            resetOOB();
        }
    }

    void determinePreviousPositionToRewindTo(ref Vector3 cameraRigPreviousPositionToRewindTo, ref Vector3 headsetPreviousPositionToRewindTo, PreventionMode preventionMode)
    {

        // There are two main branches of this function, rewind and push back
        // Push Back uses the immediately previous position
        // Rewind uses the previously-saved safe positions

        // PUSH BACK
        if (preventionMode == PreventionMode.PushBack)
        {

            // If the player is ArmSwinging, we need to push back farther in order to ensure their headset doesn't get stuck in the wall
            // So, we grab the oldest position in the cache
            if (armSwinging)
            {
                cameraRigPreviousPositionToRewindTo = previousCameraRigPushBackPositions.First.Value;
                headsetPreviousPositionToRewindTo = previousHeadsetPushBackPositions.First.Value;

                // Replace all positions with the safe ones we just found
                seedSavedPositions(cameraRigPreviousPositionToRewindTo, headsetPreviousPositionToRewindTo);
            }
            // If the player is not ArmSwinging, they are physically moving and the second-most-recent position works fine.
            else
            {

                // The last (most recent) position stored is what got us in this mess to begin with
                previousCameraRigPushBackPositions.RemoveLast();
                previousHeadsetPushBackPositions.RemoveLast();

                // The "last" position is now the one before this frame, so we know it's safe
                cameraRigPreviousPositionToRewindTo = previousCameraRigPushBackPositions.Last.Value;
                headsetPreviousPositionToRewindTo = previousHeadsetPushBackPositions.Last.Value;

                // Note that we don't drain / re-seed the cache.  This will allow us to roll back even farther if necessary.
            }
        }
        // REWIND		
        // If the headset/rig caches have at least rewindNumSavedPositionsToRewind worth of positions in the cache
        else if (previousHeadsetRewindPositions.Count >= rewindNumSavedPositionsToRewind && previousCameraRigRewindPositions.Count >= rewindNumSavedPositionsToRewind)
        {

            for (int trimCounter = 1; trimCounter < rewindNumSavedPositionsToRewind; trimCounter++)
            {
                previousHeadsetRewindPositions.RemoveLast();
                previousCameraRigRewindPositions.RemoveLast();
            }

            headsetPreviousPositionToRewindTo = previousHeadsetRewindPositions.Last.Value;
            cameraRigPreviousPositionToRewindTo = previousCameraRigRewindPositions.Last.Value;
        }
        // if the caches have less than rewindNumSavedPositionsToRewind postions, drain them to 1 and use that
        else
        {
            for (int trimCounter = 1; trimCounter < previousHeadsetRewindPositions.Count; trimCounter++)
            {
                previousHeadsetRewindPositions.RemoveLast();
                previousCameraRigRewindPositions.RemoveLast();
            }

            headsetPreviousPositionToRewindTo = previousHeadsetRewindPositions.Last.Value;
            cameraRigPreviousPositionToRewindTo = previousCameraRigRewindPositions.Last.Value;

        }
    }

    static Vector3 calculateCameraRigRewindPosition(Vector3 cameraRigPreviousPosition, Vector3 headsetPreviousPosition, Vector3 cameraRigPosition, Vector3 headsetPosition, PreventionMode preventionMode)
    {

        // We only care about the X/Z positioning of the headset
        Vector3 headsetPositionDifference = Armswing.vector3XZOnly(headsetPreviousPosition) - Armswing.vector3XZOnly(headsetPosition);

        Vector3 returnPosition = cameraRigPreviousPosition + headsetPositionDifference;

        return returnPosition;
    }

    PreventionMode calculateCurrentPreventionMode()
    {
        if (currentPreventionReason == PreventionReason.INSTANT_CLIMBING && instantHeightClimbPreventionMode == PreventionMode.PushBack ||
            currentPreventionReason == PreventionReason.INSTANT_FALLING && instantHeightFallPreventionMode == PreventionMode.PushBack ||
            currentPreventionReason == PreventionReason.HEADSET && preventWallClipMode == PreventionMode.PushBack)
        {

            // If Push Back Override is enabled and active, do a rewind instead of a push back
            // Also rewind if player is ArmSwinging
            //if ((pushBackOverride && pushBackOverrideActive && !rewindInProgress) ||
            //	armSwinging) {
            if (pushBackOverride && pushBackOverrideActive && !rewindInProgress)
            {
                return PreventionMode.Rewind;
            }
            else
            {
                return PreventionMode.PushBack;
            }
        }
        else
        {
            return PreventionMode.Rewind;
        }
    }

    void saveRewindPosition(bool force = false)
    {

        // Unconditionally save regardless of settings
        if (force)
        {
            previousCameraRigRewindPositions.AddLast(cameraRigGameObject.transform.position);
            previousHeadsetRewindPositions.AddLast(headsetGameObject.transform.position);
            lastCameraRigRewindPositionSaved = cameraRigGameObject.transform.position;
            lastHeadsetRewindPositionSaved = headsetGameObject.transform.position;
        }

        // If you're out of bounds, DEFINITELY don't want to save this position
        if (outOfBounds)
        {
            return;
        }

        Vector3 previousSavedPosInWorldUnits = lastCameraRigRewindPositionSaved + lastHeadsetRewindPositionSaved;

        // If the distance between the previous saved position and the current position is too small, bail out 
        if (isDistanceFarEnough(previousSavedPosInWorldUnits, headsetGameObject.transform.position, rewindMinDistanceChangeToSavePosition) == false)
        {
            return;
        }

        // If both Prevent Falling and Climbing features are enabled, it is important that we don't save a position to the rewind queue that could 
        // get the player stuck (too steep to go down, too steep to climb back up).  So, only save rewind positions if the ground
        // underneath the player's feet is safe to both fall down and climb up.
        // Also, if Prevent Wall Walking is enabled, don't save positions that aren't okay to wall-walk on

        // start with the assumption that the position is safe
        // if we the features and options enabled below have us check and find an unsafe position, we'll change it then
        bool isPositionSafe = true;

        // if the headset is in geometry, don't save this position
        if (preventWallClip && headsetCollider.inGeometry)
        {
            isPositionSafe = false;
        }

        // only applies if both preventFalling and preventClimbing are enabled, and if the user wants to skip unsafe positions
        if (isPositionSafe && preventFalling && preventClimbing && rewindDontSaveUnsafeClimbFallPositions)
        {
            // if the angle is greater than we can climb, not safe
            // if angle is greater than we can fall, not safe
            if ((Mathf.Abs(latestCenterChangeAngle) > preventClimbingMaxAnglePlayerCanClimb) || (Mathf.Abs(latestCenterChangeAngle) > preventFallingMaxAnglePlayerCanFall))
            {
                isPositionSafe = false;
            }
        }

        // only applies if the climb fall check above didn't find an unsafe position, if preventWallWalking is enabled, 
        // and if the user wants to skip unsafe positions
        if (isPositionSafe && preventWallWalking && rewindDontSaveUnsafeWallWalkPositions)
        {
            // if the current position is considered wall walking but the player hasn't been wall walking long enough to trigger a rewind,
            // this is an unsafe position

            // If the camera rig is moving up, compare to preventClimbingMaxAnglePlayerCanClimb
            if (cameraRigGameObject.transform.position.y > previousCameraRigRewindPositions.Last.Value.y)
            {
                if (Mathf.Abs(latestSideChangeAngle) > preventClimbingMaxAnglePlayerCanClimb)
                {
                    isPositionSafe = false;
                }
            }
            // If the camera rig is moving down, compare to preventFallingMaxAnglePlayerCanFall
            if (cameraRigGameObject.transform.position.y < previousCameraRigRewindPositions.Last.Value.y)
            {
                if (Mathf.Abs(latestSideChangeAngle) > preventFallingMaxAnglePlayerCanFall)
                {
                    isPositionSafe = false;
                }
            }
        }

        // if the assumption that the position is safe survives this long, it must be safe!
        if (isPositionSafe)
        {
            previousCameraRigRewindPositions.AddLast(cameraRigGameObject.transform.position);
            previousHeadsetRewindPositions.AddLast(headsetGameObject.transform.position);
            lastCameraRigRewindPositionSaved = cameraRigGameObject.transform.position;
            lastHeadsetRewindPositionSaved = headsetGameObject.transform.position;
        }

        // If the number of positions in the queue is > the number of rewind frames we store, pop the oldest stored position
        while (previousCameraRigRewindPositions.Count > rewindNumSavedPositionsToStore)
        {
            previousCameraRigRewindPositions.RemoveFirst();
        }
        while (previousHeadsetRewindPositions.Count > rewindNumSavedPositionsToStore)
        {
            previousHeadsetRewindPositions.RemoveFirst();
        }
    }

    void saveReason(Queue<PreventionReason> reasonQueue, PreventionReason reason, int maxListSize)
    {
        if (outOfBounds)
        {
            return;
        }

        // Store the reason
        reasonQueue.Enqueue(reason);

        // If the number of reasons in the queue is >= the number of rewind frames we store, pop the oldest stored reason
        while (reasonQueue.Count > maxListSize)
        {
            reasonQueue.Dequeue();
        }
    }

    void saveRaycastHit(List<RaycastHit> raycastHitList, RaycastHit raycastHit, int maxListSize)
    {
        // Store the raycast
        raycastHitList.Add(raycastHit);

        while (raycastHitList.Count > maxListSize)
        {
            raycastHitList.RemoveAt(0);
        }
    }

    void savePosition(LinkedList<Vector3> positionList, Vector3 position, int maxListSize)
    {
        // Store the position
        positionList.AddLast(position);

        while (positionList.Count > maxListSize)
        {
            positionList.RemoveFirst();
        }
    }

    void saveFloatToLinkedList(LinkedList<float> linkedList, float value, int maxListSize)
    {
        // Store the position
        linkedList.AddLast(value);

        while (linkedList.Count > maxListSize)
        {
            linkedList.RemoveFirst();
        }

    }

    void incrementPushBackOverride()
    {
        pushBackOverrideValue = pushBackOverrideValue + (pushBackOverrideRefillPerSec * Time.deltaTime);
        pushBackOverrideValue = Mathf.Clamp(pushBackOverrideValue, 0, pushBackOverrideMaxTokens);

        if (pushBackOverrideValue < 1)
        {
            pushBackOverrideActive = true;
        }
        else
        {
            pushBackOverrideActive = false;
        }
    }

    void decrementPushBackOverride()
    {
        pushBackOverrideValue = pushBackOverrideValue - 1;
        pushBackOverrideValue = Mathf.Clamp(pushBackOverrideValue, 0f, pushBackOverrideMaxTokens);

        if (pushBackOverrideValue < 1)
        {
            pushBackOverrideActive = true;
        }
    }

    // Returns the average of two Quaternions
    Quaternion averageRotation(Quaternion rot1, Quaternion rot2)
    {
        return Quaternion.Slerp(rot1, rot2, 0.5f);
    }

    // Fade the screen to black
    void fadeOut()
    {
        // SteamVR_Fade is too fast in builds.  We compenstate for this here.
#if UNITY_EDITOR
        SteamVR_Fade.View(Color.black, rewindFadeOutSec);
#else
				SteamVR_Fade.View(Color.black, rewindFadeOutSec * .666f);
#endif
    }

    // Fade the screen back to clear
    void fadeIn()
    {
        // SteamVR_Fade is too fast in builds.  We compenstate for this here.
#if UNITY_EDITOR
        SteamVR_Fade.View(Color.clear, rewindFadeInSec);
#else
				SteamVR_Fade.View(Color.clear, rewindFadeInSec * .666f);
#endif
    }

    // Returns a Vector3 with only the X and Z components (Y is 0'd)
    public static Vector3 vector3XZOnly(Vector3 vec)
    {
        return new Vector3(vec.x, 0f, vec.z);
    }

    // Returns a forward vector given the distance and direction
    public static Vector3 getForwardXZ(float forwardDistance, Quaternion direction)
    {
        Vector3 forwardMovement = direction * Vector3.forward * forwardDistance;
        return vector3XZOnly(forwardMovement);
    }

    // Returns the average Y value of a RaycastHit list
    public static float averageRaycastHitY(List<RaycastHit> list)
    {

        float avgY = 0;

        foreach (RaycastHit item in list)
        {
            avgY += item.point.y;
        }

        avgY /= list.Count;

        return avgY;
    }

    // Returns the average rotation of the two controllers
    Quaternion determineAverageControllerRotation()
    {
        // Build the average rotation of the controller(s)
        Quaternion newRotation;

        // Both controllers are present
        if (m_LeftController != null && m_RightController != null)
        {
            newRotation = averageRotation(leftControllerGameObject.transform.rotation, rightControllerGameObject.transform.rotation);
        }
        // Left controller only
        else if (m_LeftController != null && m_RightController == null)
        {
            newRotation = leftControllerGameObject.transform.rotation;
        }
        // Right controller only
        else if (m_RightController != null && m_LeftController == null)
        {
            newRotation = rightControllerGameObject.transform.rotation;
        }
        // No controllers!
        else
        {
            newRotation = Quaternion.identity;
        }

        return newRotation;
    }

    float smoothedControllerMovement(LinkedList<float> controllerMovementHistory)
    {

        // If the controllerMovementHistory has a length of 1, just return that value
        if (controllerMovementHistory.Count == 1)
        {
            return controllerMovementHistory.First.Value;
        }

        float high = controllerMovementHistory.First.Value;
        float total = 0;

        foreach (float val in controllerMovementHistory)
        {
            total += val;
            if (val > high)
            {
                high = val;
            }
        }

        return ((total - high) / (controllerMovementHistory.Count - 1));

    }

    // Thanks to cjdev from "http://answers.unity3d.com/questions/564166/how-to-find-perpendicular-line-in-2d.html"
    Vector3 determineSideRayOriginXZ(RaycastHit thisRaycastHit, RaycastHit lastRaycastHit, float offsetDistance)
    {

        Vector2 p1 = new Vector2(thisRaycastHit.point.x, thisRaycastHit.point.z);
        Vector2 p2 = new Vector2(lastRaycastHit.point.x, lastRaycastHit.point.z);

        Vector2 v = p2 - p1;

        Vector2 p3 = new Vector2(-v.y, v.x) / Mathf.Sqrt(Mathf.Pow(v.x, 2) + Mathf.Pow(v.y, 2)) * -offsetDistance;
        p3 += p1;

        Vector3 sideRayOrigin = new Vector3(p3.x, 0f, p3.y);

        return sideRayOrigin;
    }

    bool isDistanceFarEnough(Vector3 position1, Vector3 position2, float minDistance)
    {
        return (Vector3.Distance(position1, position2) >= minDistance);
    }

    // Adds the Collider script to the headset if it doesn't already exist
    void setupHeadsetCollider()
    {
        headsetCollider = headsetGameObject.GetComponent<HeadsetCollider>();
        if (!headsetCollider)
        {
            headsetCollider = headsetGameObject.AddComponent<HeadsetCollider>();

        }
        headsetCollider.setLayerMask(preventWallClipLayerMask);
        headsetCollider.setHeadsetSphereColliderRadius(preventWallClipHeadsetColliderRadius);
        headsetCollider.setMinAngleWallClipForOOB(preventWallClipMinAngleToTrigger);
    }

    static float calculateMovement(AnimationCurve curve, float change, float maxInput, float maxSpeed)
    {
        float changeInWUPS = change / Time.deltaTime;
        float movement = Mathf.Lerp(0, maxSpeed, curve.Evaluate(changeInWUPS / maxInput)) * Time.deltaTime;

        return movement;
    }

    RaycastHit raycast(Vector3 origin, Vector3 direction, float maxDistance, LayerMask layerMask, out bool didRaycastHit)
    {
        Ray ray = new Ray(origin, direction);
        RaycastHit raycastHit;
        didRaycastHit = Physics.Raycast(ray, out raycastHit, maxDistance, layerMask);

        return raycastHit;
    }

    RaycastHit raycast(Vector3 origin, Vector3 direction, float maxDistance, LayerMask layerMask)
    {
        bool dummyBool;
        return raycast(origin, direction, maxDistance, layerMask, out dummyBool);
    }

    /***** CLEANUP *****/
    void resetOOB()
    {
        if (!rewindInProgress)
        {
            currentPreventionReason = PreventionReason.NONE;
            outOfBounds = false;
        }
        if (pushBackOverrideActive)
        {
            pushBackOverrideValue = pushBackOverrideMaxTokens;
        }
    }

    void resetReasonHistory()
    {
        climbFallPreventionReasonHistory.Clear();
        wallWalkPreventionReasonHistory.Clear();
    }

    void resetRaycastHitHistory()
    {
        headsetCenterRaycastHitHistoryHeight.Clear();
        headsetCenterRaycastHitHistoryPrevention.Clear();
    }

    void resetSavedPositions()
    {
        previousCameraRigPushBackPositions.Clear();
        previousCameraRigPushBackPositions.Clear();
    }

    void seedRaycastHitHistory()
    {
        bool didRaycastHit = false;
        RaycastHit raycastHit = raycast(headsetGameObject.transform.position, Vector3.down, raycastMaxLength, raycastGroundLayerMask, out didRaycastHit);

        if (didRaycastHit)
        {
            saveRaycastHit(headsetCenterRaycastHitHistoryHeight, raycastHit, 1);
        }
    }

    void seedSavedPositions()
    {
        for (int count = 0; count < previousPushbackPositionSize; count++)
        {
            savePosition(previousCameraRigPushBackPositions, cameraRigGameObject.transform.position, previousPushbackPositionSize);
            savePosition(previousHeadsetPushBackPositions, headsetGameObject.transform.position, previousPushbackPositionSize);
        }
    }

    void seedSavedPositions(Vector3 cameraRigPosition, Vector3 headsetPosition)
    {
        for (int count = 0; count < previousPushbackPositionSize; count++)
        {
            savePosition(previousCameraRigPushBackPositions, cameraRigPosition, previousPushbackPositionSize);
            savePosition(previousHeadsetPushBackPositions, headsetPosition, previousPushbackPositionSize);
        }
    }


    void resetRewindPositions()
    {
        previousCameraRigRewindPositions.Clear();
        previousHeadsetRewindPositions.Clear();
    }

    /***** PUBLIC FUNCTIONS *****/
    // Moves the camera to another world position without a rewind
    // Also resets all caches and saved variables to prevent false OOB events
    // Allows other scripts and ArmSwinger mechanisms to artifically move the player without a rewind happening
    public void moveCameraRig(Vector3 newPosition, PreventionReason reason = PreventionReason.MANUAL)
    {
        outOfBounds = true;
        currentPreventionReason = reason;

        // Reset all caches
        resetReasonHistory();
        resetRaycastHitHistory();
        latestArtificialMovement = 0f;
        latestArtificialRotation = Quaternion.identity;

        resetRewindPositions();
        resetSavedPositions();

        cameraRigGameObject.transform.position = newPosition;

        seedRaycastHitHistory();
        seedSavedPositions();

        outOfBounds = false;
        currentPreventionReason = PreventionReason.NONE;

        saveRewindPosition(true);
        if (raycastOnlyHeightAdjustWhileArmSwinging)
        {
            lastRaycastHitWhileArmSwinging = raycast(headsetGameObject.transform.position, Vector3.down, raycastMaxLength, raycastGroundLayerMask);
        }

        // If raycastOnlyHeightAdjustWhileArmSwinging is enabled or if play area height adjustment is paused, we need to force an adjustment of the camera rig
        if (raycastOnlyHeightAdjustWhileArmSwinging || playAreaHeightAdjustmentPaused)
        {
            adjustCameraRig();
        }
    }

    /***** GET SET *****/
    public float armSwingBothControllersCoefficient
    {
        get
        {
            return _armSwingBothControllersCoefficient;
        }

        set
        {
            float min = 0f;
            float max = 10f;

            if (value >= min && value <= max)
            {
                _armSwingBothControllersCoefficient = value;
            }
            else
            {
                Debug.LogWarning("ArmSwinger:armSwingBothControllersCoefficient:: Requested new value " + value + " is out of range (" + min + ".." + max + ")");
            }
        }

    }

    public float armSwingSingleControllerCoefficient
    {
        get
        {
            return _armSwingSingleControllerCoefficient;
        }

        set
        {
            float min = 0f;
            float max = 10f;

            if (value >= min && value <= max)
            {
                _armSwingSingleControllerCoefficient = value;
            }
            else
            {
                Debug.LogWarning("ArmSwinger:armSwingSingleControllerCoefficient:: Requested new value " + value + " is out of range (" + min + ".." + max + ")");
            }
        }
    }

    public int raycastAverageHeightCacheSize
    {
        get
        {
            return _raycastAverageHeightCacheSize;
        }

        set
        {
            int min = 2;
            int max = 90;

            if (value >= min && value <= max)
            {
                _raycastAverageHeightCacheSize = value;
            }
            else
            {
                Debug.LogWarning("ArmSwinger:raycastAverageHeightCacheSize:: Requested new value " + value + " is out of range (" + min + ".." + max + ")");
            }
        }
    }

    public bool preventWallClip
    {
        get
        {
            return _preventWallClip;
        }

        set
        {
            _preventWallClip = value;
            if (_preventWallClip)
            {
                setupHeadsetCollider();
            }
        }
    }

    public float preventWallClipMinAngleToTrigger
    {
        get
        {
            return _preventWallClipMinAngleToTrigger;
        }

        set
        {
            float min = 0f;
            float max = 90f;

            if (value >= min && value <= max)
            {
                _preventWallClipMinAngleToTrigger = value;
                headsetCollider.setMinAngleWallClipForOOB(value);
            }
            else
            {
                Debug.LogWarning("ArmSwinger:preventWallClipMinAngleToTrigger:: Requested new value " + value + " is out of range (" + min + ".." + max + ")");
            }
        }
    }

    public float preventClimbingMaxAnglePlayerCanClimb
    {
        get
        {
            return _preventClimbingMaxAnglePlayerCanClimb;
        }

        set
        {
            float min = 0f;
            float max = 90f;

            if (value >= min && value <= max)
            {
                _preventClimbingMaxAnglePlayerCanClimb = value;
            }
            else
            {
                Debug.LogWarning("ArmSwinger:preventClimbingMaxAnglePlayerCanClimb:: Requested new value " + value + " is out of range (" + min + ".." + max + ")");
            }
        }
    }

    public float preventFallingMaxAnglePlayerCanFall
    {
        get
        {
            return _preventFallingMaxAnglePlayerCanFall;
        }

        set
        {
            float min = 0f;
            float max = 90f;

            if (value >= min && value <= max)
            {
                _preventFallingMaxAnglePlayerCanFall = value;
            }
            else
            {
                Debug.LogWarning("ArmSwinger:preventFallingMaxAnglePlayerCanFall:: Requested new value " + value + " is out of range (" + min + ".." + max + ")");
            }
        }
    }

    public PreventionReason currentPreventionReason
    {
        get
        {
            return _currentPreventionReason;
        }

        set
        {
            _currentPreventionReason = value;
        }
    }

    public bool preventionsPaused
    {
        get
        {
            return _preventionsPaused;
        }
        set
        {
            // If the pause is being set to false, use moveCameraRig to reset and seed necessary caches
            if (value == false)
            {
                moveCameraRig(cameraRigGameObject.transform.position);
            }
            _preventionsPaused = value;
        }
    }

    public bool anglePreventionsPaused
    {
        get
        {
            return _anglePreventionsPaused;
        }
        set
        {
            // If the pause is being set to false, use moveCameraRig to reset and seed necessary caches
            if (value == false)
            {
                moveCameraRig(cameraRigGameObject.transform.position);
            }
            _anglePreventionsPaused = value;
        }
    }

    public bool wallClipPreventionPaused
    {
        get
        {
            return _wallClipPreventionPaused;
        }
        set
        {
            _wallClipPreventionPaused = value;
        }
    }

    public bool playAreaHeightAdjustmentPaused
    {
        get
        {
            return _playAreaHeightAdjustmentPaused;
        }
        set
        {
            // If the pause is being set to false, use moveCameraRig to reset and seed necessary caches
            if (value == false)
            {
                moveCameraRig(cameraRigGameObject.transform.position);
            }
            _playAreaHeightAdjustmentPaused = value;
        }
    }

    public bool armSwingingPaused
    {
        get
        {
            return _armSwingingPaused;
        }
        set
        {
            // If the pause is being set to false, reset the cached movement values so that unexpected inertia does not occur
            if (value == false)
            {
                latestArtificialMovement = 0f;
                latestArtificialRotation = Quaternion.identity;
            }
            _armSwingingPaused = value;
        }
    }

    public PreventionMode currentPreventionMode
    {
        get
        {
            return calculateCurrentPreventionMode();
        }
    }

    public float armSwingControllerSpeedForMaxSpeed
    {
        get
        {
            return _armSwingControllerSpeedForMaxSpeed;
        }
        set
        {
            _armSwingControllerSpeedForMaxSpeed = value;
        }
    }

    public float armSwingMaxSpeed
    {
        get
        {
            return _armSwingMaxSpeed * cameraRigScaleModifier;
        }
        set
        {
            _armSwingMaxSpeed = value;
        }
    }

    public float raycastMaxLength
    {
        get
        {
            return _raycastMaxLength * cameraRigScaleModifier;
        }
        set
        {
            _raycastMaxLength = value;
        }
    }

    public float preventWallClipHeadsetColliderRadius
    {
        get
        {
            return _preventWallClipHeadsetColliderRadius;
        }
        set
        {
            _preventWallClipHeadsetColliderRadius = value;
        }
    }

    public float instantHeightMaxChange
    {
        get
        {
            return _instantHeightMaxChange * cameraRigScaleModifier;
        }
        set
        {
            _instantHeightMaxChange = value;
        }
    }

    public float rewindMinDistanceChangeToSavePosition
    {
        get
        {
            return _rewindMinDistanceChangeToSavePosition * cameraRigScaleModifier;
        }
        set
        {
            _rewindMinDistanceChangeToSavePosition = value;
        }
    }

    public bool movingInertia
    {
        get
        {
            return _movingInertia;
        }
        set
        {
            latestArtificialMovement = 0f;
            _movingInertia = value;
        }
    }

    public bool stoppingInertia
    {
        get
        {
            return _stoppingInertia;
        }
        set
        {
            latestArtificialMovement = 0f;
            _stoppingInertia = value;
        }
    }
}

public class HeadsetCollider : MonoBehaviour
{

    /////////////////////
    // CLASS VARIABLES //
    /////////////////////

    //// Public variables ////
    public float headsetSphereColliderRadius;
    public bool inGeometry = false;

    //// Public objects ////

    //// Private variables ////
    private float minAngleToRewindDueToWallClip;

    //// Private objects ////
    private Armswing armSwinger;

    private SphereCollider headsetSphereCollider;
    private Rigidbody headsetRigidbody;

    private LayerMask groundRayLayerMask;

    // Track previous collision position
    private Vector3 previousCollisionPosition;
    private Quaternion previousCollisionRotation;

    //////////////////////
    // INITIATILIZATION //
    //////////////////////
    void Start()
    {
        // Create a box collider for the headset if it doesn't already exist
        // The collider is a non-trigger
        headsetSphereCollider = GetComponent<SphereCollider>();
        if (!headsetSphereCollider)
        {
            headsetSphereCollider = this.gameObject.AddComponent<SphereCollider>();
            headsetSphereCollider.isTrigger = false;
            headsetSphereCollider.radius = headsetSphereColliderRadius;
        }
        else
        {
            if (headsetSphereCollider.isTrigger == true)
            {
                Debug.LogError("HeadsetCollider.Start():: There is already a sphere collider on your headset, but it is a trigger. Prevent Wall Clip will fail.");
            }

            Debug.LogWarning("HeadsetCollider.Start():: There is already a sphere collider on your headset.  Please ensure that it is an appropriate radius for Prevent Wall Clip to work.");
        }

        // Create a rigidbody for the headset if it doesn't already exit
        // The rigidbody is non-kinematic, and frozen for position and rotation
        // This was done to allow OnCollisionEnter's collision.point to work when detecting the normals of surfaces the headset runs into
        // If you have a better way to do this, please PLEASE submit it.  This solution makes me sad.
        headsetRigidbody = GetComponent<Rigidbody>();
        if (!headsetRigidbody)
        {
            headsetRigidbody = this.gameObject.AddComponent<Rigidbody>();
            headsetRigidbody.isKinematic = false;
            headsetRigidbody.useGravity = false;
            headsetRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            if (headsetRigidbody.isKinematic == true)
            {
                Debug.LogError("HeadsetCollider.Start():: There is already a RigidBody on your headset, but it is kinematic.  Prevent Wall Clip will fail.");
            }
            if (headsetRigidbody.constraints != RigidbodyConstraints.FreezeAll)
            {
                Debug.LogWarning("HeadsetCollider.Start():: There is already a RigidBody on your headset, but it does not have all constraints frozen.  Physics with the headset may be erratic.");
            }
        }

        // Initial collision position
        previousCollisionPosition = this.transform.position;
        previousCollisionRotation = this.transform.rotation;

        armSwinger = GameObject.FindObjectOfType<Armswing>();

    }

    ///////////////////
    // MONOBEHAVIOUR //
    ///////////////////
    void OnCollisionEnter(Collision collision)
    {

        // Never collide with any SteamVR tracked objects
        if (collision.gameObject.GetComponent<SteamVR_TrackedObject>())
        {
            return;
        }

        if (armSwinger.preventWallClip == false || armSwinger.wallClipPreventionPaused || armSwinger.preventionsPaused)
        {
            return;
        }

        // If we're already in geometry, unconditionally push back
        if (inGeometry)
        {
            armSwinger.triggerRewind(Armswing.PreventionReason.HEADSET);
            armSwinger.wallClipThisFrame = true;
            return;
        }

        if (armSwinger.currentPreventionReason == Armswing.PreventionReason.NONE || armSwinger.currentPreventionReason == Armswing.PreventionReason.HEADSET)
        {
            if ((groundRayLayerMask.value & 1 << collision.gameObject.layer) != 0)
            {

                foreach (ContactPoint contactPoint in collision.contacts)
                {
                    Vector3 normalOfCollisionPoint = contactPoint.normal;

                    float angleOfCollisionPoint = Vector3.Angle(Vector3.up, normalOfCollisionPoint);

                    //Debug.Log("this.transform=" + this.transform);
                    //Debug.Log("previousCollisionTransform=" + previousCollisionTransform);

                    if (this.transform.position == previousCollisionPosition && this.transform.rotation == previousCollisionRotation)
                    {
                        return;
                    }

                    if (angleOfCollisionPoint >= minAngleToRewindDueToWallClip)
                    {
                        inGeometry = true;
                        armSwinger.triggerRewind(Armswing.PreventionReason.HEADSET);
                        armSwinger.wallClipThisFrame = true;
                        previousCollisionPosition = this.transform.position;
                        previousCollisionRotation = this.transform.rotation;
                        return;
                    }
                }
            }
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        inGeometry = false;
    }

    /////////////
    // COMPUTE //
    /////////////

    /////////
    // GET //
    /////////

    /////////
    // SET //
    /////////
    public void setLayerMask(LayerMask newMask)
    {
        groundRayLayerMask = newMask;
    }

    public void setHeadsetSphereColliderRadius(float newRadius)
    {
        headsetSphereColliderRadius = newRadius;
    }

    public void setMinAngleWallClipForOOB(float newAngle)
    {
        minAngleToRewindDueToWallClip = newAngle;
    }

}
