using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Smooth_locomotion : MonoBehaviour
{
    public float Gravity = 98.0f;
    public float Sensitivity = 0.1f;
    public float MaxSpeed = 1.0f;
    public float RotateIncrement = 90;

    public SteamVR_Action_Boolean RotatePress = null;
    public SteamVR_Action_Boolean MovePress = null;
    public SteamVR_Action_Vector2 MoveValue = null;

    public Transform Player;

    private float _speed;

    private CharacterController _characterController;
    private Transform _head;

    private void Awake()
    {
        _characterController = Player.GetComponent<CharacterController>();
    }

    private void Start()
    {
        _head = SteamVR_Render.Top().head;
    }

    private void Update()
    {
        //HandleHeight();
        CalculateMovement();
    }

    private void HandleHeight()
    {
        // Get the head in local space 
        var headHeight = Mathf.Clamp(_head.localPosition.y, 1, 2);
        _characterController.height = headHeight;

        // Cut in half
        var newCenter = Vector3.zero;
        newCenter.y = _characterController.height / 2;
        newCenter.y += _characterController.skinWidth;

        // Move capsule in local space
        newCenter.x = _head.localPosition.x;
        newCenter.z = _head.localPosition.z;

        // Apply
        _characterController.center = newCenter;
    }

    private void CalculateMovement()
    {
        // Figure out movement orientation 
        var orientationEuler = new Vector3(0, _head.eulerAngles.y, 0);
        var orientation = Quaternion.Euler(orientationEuler);
        var movement = Vector3.zero;

        // If not moving
        if(MovePress.GetStateUp(SteamVR_Input_Sources.LeftHand))
            _speed = 0;

        if (MovePress.state)
        {
            // Add, clamp
            _speed += MoveValue.axis.y * Sensitivity;
            _speed = Mathf.Clamp(_speed, -MaxSpeed, MaxSpeed);

            // Orientation
            movement += orientation * (_speed * Vector3.forward);
        }

        // Gravity
        movement.y -= Gravity * Time.deltaTime;

        // Apply
        _characterController.Move(movement * Time.deltaTime);
    }

    private void SnapRotation()
    {
        var snapValue = 0.0f;

        if (RotatePress.GetStateDown(SteamVR_Input_Sources.LeftHand))
            snapValue = -Mathf.Abs(RotateIncrement);
        if (RotatePress.GetStateDown(SteamVR_Input_Sources.RightHand))
            snapValue = Mathf.Abs(RotateIncrement);

        Player.transform.RotateAround(_head.position, Player.up, snapValue);
    }
}
