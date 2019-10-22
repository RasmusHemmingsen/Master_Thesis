﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Smooth_locomotion : MonoBehaviour
{
    public float m_Sensitivity = 0.1f;
    public float m_MaxSpeed = 1.0f;

    public SteamVR_Action_Boolean m_MovePress = null;
    public SteamVR_Action_Vector2 m_MoveValue = null;

    public Transform m_Player;

    private float m_Speed = 0.0f;

    private CharacterController m_CharacterController = null;
    private Transform m_CameraRig = null;
    private Transform m_Head = null;

    private void Awake()
    {
        m_CharacterController = m_Player.GetComponent<CharacterController>();
 
        m_MovePress[SteamVR_Input_Sources.LeftHand].onStateDown += Move;
    }

    private void Start()
    {
        m_CameraRig = SteamVR_Render.Top().origin;
        m_Head = SteamVR_Render.Top().head;
    }

    private void Update()
    {
        HandleHead();
        HandleHeight();
        if (!m_CharacterController.isGrounded)
        {
            m_CharacterController.Move(-m_Player.up * 9.8f);
        }
    }

    private void HandleHead()
    {
        // Store current
        Vector3 oldPosition = m_CameraRig.position;
        Quaternion oldRotation = m_CameraRig.rotation;

        // Rotation
        m_Player.eulerAngles = new Vector3(0.0f, m_Head.rotation.eulerAngles.y, 0.0f);

        // Restore
        m_CameraRig.position = oldPosition;
        m_CameraRig.rotation = oldRotation;
    }

    private void Move(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        do
        {
            // Figure out movement orientation 
            Vector3 orientationEuler = new Vector3(0, m_Player.eulerAngles.y, 0);
            Quaternion orientation = Quaternion.Euler(orientationEuler);
            Vector3 movement = Vector3.zero;

            // Add, clamp
            m_Speed += m_MoveValue.axis.y * m_Sensitivity;
            m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed);

            // Orientation
            movement += orientation * (m_Speed * Vector3.forward) * Time.deltaTime;

            // Apply
            m_CharacterController.Move(movement);

        } while (m_MovePress[SteamVR_Input_Sources.LeftHand].state);

        m_Speed = 0;
    }

    private void HandleHeight()
    {
        // Get the head in local space 
        float headHight = Mathf.Clamp(m_Head.localPosition.y, 1, 2);
        m_CharacterController.height = headHight;

        // Cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = m_CharacterController.height / 2;
        newCenter.y += m_CharacterController.skinWidth;

        // Move capsule in local space
        newCenter.x = m_Head.localPosition.x;
        newCenter.z = m_Head.localPosition.z;

        // Rotate
        newCenter = Quaternion.Euler(0, -m_Player.eulerAngles.y, 0) * newCenter;

        // Apply
        m_CharacterController.center = newCenter;
    }
}
