using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;

    private SteamVR_Behaviour_Pose m_Pose;
    private FixedJoint m_Joint;

    private Interactable m_CurrentInteractable = null;
    public List<Interactable> m_ContactInteractables = new List<Interactable>();

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();

        // Down 
        m_GrabAction[SteamVR_Input_Sources.Any].onStateDown += Pickup;

        // Up
        m_GrabAction[SteamVR_Input_Sources.Any].onStateUp += Drop;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable") ||
            other.gameObject.CompareTag("Door") ||
            other.gameObject.CompareTag("Button"))
        {
            m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());
        }

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable") ||
            other.gameObject.CompareTag("Door") ||
            other.gameObject.CompareTag("Button"))
        {
            m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
        }
    }

    public void GrapClicked(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        // Get nearest
        m_CurrentInteractable = GetNearestInteractable();

        // Null check
        if (!m_CurrentInteractable)
            return;

        if (m_CurrentInteractable.CompareTag("Interactable"))
            Pickup(action, source);
        else if (m_CurrentInteractable.CompareTag("Door"))
            Pickup(action, source);
    }

    public void Door(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        Vector3 targetDelta = transform.position - m_CurrentInteractable.transform.position;
        targetDelta.y = 0;

        float AngleDiff = Vector3.Angle(m_CurrentInteractable.transform.forward, targetDelta);

        Vector3 cross = Vector3.Cross(m_CurrentInteractable.transform.forward, targetDelta);

        
    }

    public void Pickup(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        // Already held, check
        if (m_CurrentInteractable.m_ActiveHand)
            m_CurrentInteractable.m_ActiveHand.Drop(action, source);

        // Position
        m_CurrentInteractable.transform.position = transform.position;

        // Attach
        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        m_Joint.connectedBody = targetBody;

        // Set active hand
        m_CurrentInteractable.m_ActiveHand = this;
    }

    public void Drop(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        // Null check
        if (!m_CurrentInteractable)
            return;

        // Apply Velocity
        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        targetBody.velocity = m_Pose.GetVelocity();
        targetBody.angularVelocity = m_Pose.GetAngularVelocity();

        // Detach
        m_Joint.connectedBody = null;

        // Clear
        m_CurrentInteractable.m_ActiveHand = null;
        m_CurrentInteractable = null;
    }

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0.0f;

        foreach(Interactable interactable in m_ContactInteractables)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;

            if(distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }

        return nearest;
    }
}
