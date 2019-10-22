using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;
    public SteamVR_Action_Vibration m_Haptic = null;

    private Interactable m_CurrentInteractable = null;
    public List<Interactable> m_ContactInteractables = new List<Interactable>();

    private void Awake()
    {
        // Down 
        m_GrabAction[SteamVR_Input_Sources.RightHand].onStateDown += Pickup;
        m_GrabAction[SteamVR_Input_Sources.LeftHand].onStateDown += Pickup;

        // Up
        m_GrabAction[SteamVR_Input_Sources.RightHand].onStateUp += Drop;
        m_GrabAction[SteamVR_Input_Sources.LeftHand].onStateUp += Drop;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;

        m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());   
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;

        m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
    }


    public void Pickup(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        // Get nearest
        m_CurrentInteractable = GetNearestInteractable();

        // Null check
        if (!m_CurrentInteractable)
            return;

        // Already held, check
        if (m_CurrentInteractable.m_ActiveHand)
            m_CurrentInteractable.m_ActiveHand.Drop(action, source);

        // Set active hand
        m_CurrentInteractable.m_ActiveHand = this;

        m_CurrentInteractable.PickUp();

        Pulse(0.5f, 150, 60, source);
    }

    public void Drop(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        // Null check
        if (!m_CurrentInteractable)
            return;

        m_CurrentInteractable.Drop();

        m_CurrentInteractable = null;
    }

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        float minDistance = float.MaxValue;
        foreach (Interactable interactable in m_ContactInteractables)
        {
            float distance = (interactable.transform.position - transform.position).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }

        return nearest;
    }

    private void Pulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources source)
    {
        m_Haptic.Execute(0, duration, frequency, amplitude, source);
    }
}
