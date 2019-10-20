using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Teleporter : MonoBehaviour
{
    public SteamVR_Action_Boolean m_TeleportAction = null;
    public GameObject m_Player;
    public Transform m_Controller;

    [HideInInspector]
    public GameObject m_pointer = null;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private bool m_HasPosition = false;
    private bool m_IsTeleporting = false;
    private float m_FadeTime = 0.5f;
    private float m_TeleportMaxRange = 5.0f;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();

        m_TeleportAction[SteamVR_Input_Sources.Any].onStateUp += TryTeleport;
    }
 
    private void Update()
    {
        // pointer
        m_HasPosition = UpdatePointer();
        m_pointer.SetActive(m_HasPosition);

    }

    private void TryTeleport(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        // Check for valid position, and if already teleporting
        if (!m_HasPosition || m_IsTeleporting)
            return;

        // Get camera rig, and head position
        Transform cameraRig = SteamVR_Render.Top().origin;
        Vector3 headPosition = SteamVR_Render.Top().head.position;

        // Figure out translation
        Vector3 groundPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);
        Vector3 translateVector = m_pointer.transform.position - groundPosition;

        // Move 
        StartCoroutine(Move(m_Player.transform, translateVector));

    }

    private IEnumerator Move(Transform player, Vector3 translation)
    {
        // Flag 
        m_IsTeleporting = true;

        // Fade to black
        SteamVR_Fade.Start(Color.black, m_FadeTime, true);

        // Apply translation 
        yield return new WaitForSeconds(m_FadeTime);
        player.position += translation;

        // Fade to clear
        SteamVR_Fade.Start(Color.clear, m_FadeTime, true);

        // De-flag
        m_IsTeleporting = false;
    }

    private bool UpdatePointer()
    {
        RaycastHit hit;

        // Ray from the controller
        Ray ray = new Ray(m_Controller.position, m_Controller.forward);

        // If it is a hit
        if(Physics.Raycast(ray, out hit) && hit.distance < m_TeleportMaxRange && hit.collider.CompareTag("CanTeleport"))
        {
            m_pointer.transform.position = hit.point;
            return true;
        }

        // If not a hit
        return false;
    }
}
