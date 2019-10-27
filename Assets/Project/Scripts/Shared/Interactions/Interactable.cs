using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    [HideInInspector]
    public Hand ActiveHand;

    [HideInInspector]
    public Rigidbody Simulator;

    public float DisconnectDistance = 0f;

    public UnityEvent PickUpEvent;
    public UnityEvent DropEvent;

    private Transform _oldParent;

    private void Start()
    {
        Simulator = new GameObject().AddComponent<Rigidbody>();
        Simulator.name = "simulator";
        Simulator.transform.parent = transform.parent;
        Simulator.useGravity = false;
    }

    private void Update()
    {
        if (ActiveHand == null) return;
        Simulator.velocity = (ActiveHand.transform.position - Simulator.position) * 50f;

        if (!(DisconnectDistance > 0f)) return;
        if(!GetComponent<Collider>().bounds.Contains(ActiveHand.transform.position) ||
           Vector3.Distance(GetComponent<Collider>().bounds.ClosestPoint(ActiveHand.transform.position),
               ActiveHand.transform.position) > DisconnectDistance)
        {
            Drop();
        }
    }

    public void PickUp()
    {
        PickUpEvent.Invoke();
        if (PickUpEvent.GetPersistentEventCount() == 0)
        {
            DefaultPickUp();
        }
    }

    public void Drop()
    {
        DropEvent.Invoke();
        if (DropEvent.GetPersistentEventCount() == 0)
        {
            DefaultDrop();
        }
        ActiveHand = null;
    }

    public void DefaultPickUp()
    {
        _oldParent = transform.parent;
        transform.parent = ActiveHand.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        GetComponent<Rigidbody>().isKinematic = true;
    }
    
    public void DefaultDrop()
    {
        transform.parent = _oldParent;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().velocity = Simulator.velocity;
        ActiveHand = null;     
    }
}
