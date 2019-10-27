using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean GrabAction = null;
    public SteamVR_Action_Vibration Haptic = null;

    private Interactable _currentInteractable = null;
    public List<Interactable> ContactInteractables = new List<Interactable>();

    private void Awake()
    {
        // Down 
        GrabAction[SteamVR_Input_Sources.RightHand].onStateDown += Pickup;
        GrabAction[SteamVR_Input_Sources.LeftHand].onStateDown += Pickup;

        // Up
        GrabAction[SteamVR_Input_Sources.RightHand].onStateUp += Drop;
        GrabAction[SteamVR_Input_Sources.LeftHand].onStateUp += Drop;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;

        ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());   
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;

        ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
    }


    public void Pickup(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        // Get nearest
        _currentInteractable = GetNearestInteractable();

        // Null check
        if (!_currentInteractable)
            return;

        // Already held, check
        if (_currentInteractable.ActiveHand)
            _currentInteractable.ActiveHand.Drop(action, source);

        // Set active hand
        _currentInteractable.ActiveHand = this;

        _currentInteractable.PickUp();

        Pulse(0.5f, 150, 60, source);
    }

    public void Drop(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        // Null check
        if (!_currentInteractable)
            return;

        _currentInteractable.Drop();

        _currentInteractable = null;
    }

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        var minDistance = float.MaxValue;
        foreach (var interactable in ContactInteractables)
        {
            var distance = (interactable.transform.position - transform.position).sqrMagnitude;
            if (!(distance < minDistance)) continue;
            minDistance = distance;
            nearest = interactable;
        }

        return nearest;
    }

    private void Pulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources source)
    {
        Haptic.Execute(0, duration, frequency, amplitude, source);
    }
}
