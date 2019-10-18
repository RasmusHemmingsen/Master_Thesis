using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    [HideInInspector]
    public Hand m_ActiveHand;

    [HideInInspector]
    public Rigidbody simulator;

    public UnityEvent m_PickUp;
    public UnityEvent m_Drop;

    private void Start()
    {
        simulator = new GameObject().AddComponent<Rigidbody>();
        simulator.name = "simulator";
        simulator.transform.parent = transform.parent;
        simulator.useGravity = false;
    }

    private void Update()
    {
        if(m_ActiveHand != null)
        {
            simulator.velocity = (m_ActiveHand.transform.position - simulator.position) * 50f;
        }
    }

    public void PickUp()
    {
        m_PickUp.Invoke();
        if (m_PickUp.GetPersistentEventCount() == 0)
        {
            DefaultPickUp();
        }
    }

    public void Drop()
    {
        m_Drop.Invoke();
        if (m_Drop.GetPersistentEventCount() == 0)
        {
            DefaultDrop();
        }
        m_ActiveHand = null;
    }

    public void DefaultPickUp()
    {
        transform.parent = m_ActiveHand.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        GetComponent<Rigidbody>().isKinematic = false;
    }
    
    public void DefaultDrop()
    {
        transform.parent = null;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().velocity = simulator.velocity;
        m_ActiveHand = null;     
    }
}
