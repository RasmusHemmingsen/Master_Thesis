using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(HingeJoint))]
public class Door : MonoBehaviour
{
    public Transform m_Parent;

    public float m_MinRotation;
    public float m_MaxRotation;

    // Start is called before the first frame update
    void Start()
    {
        JointLimits limits = new JointLimits();
        limits.min = m_MinRotation;
        limits.max = m_MaxRotation;
        GetComponent<HingeJoint>().limits = limits;
        GetComponent<HingeJoint>().useLimits = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_Parent != null)
        {
            Vector3 targetDelta = m_Parent.position - transform.position;
            targetDelta.y = 0;

            float AngleDiff = Vector3.Angle(transform.forward, targetDelta);

            Vector3 cross = Vector3.Cross(transform.forward, targetDelta);

            GetComponent<Rigidbody>().angularVelocity = cross * AngleDiff * 50f;

        }
    }
    
    public void Pickup()
    {
        m_Parent = GetComponent<Interactable>().m_ActiveHand.transform;
    }

    public void Drop()
    {
        m_Parent = null;
    }
}
