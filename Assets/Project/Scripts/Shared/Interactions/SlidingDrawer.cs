using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class SlidingDrawer : MonoBehaviour
{
    Transform parent;
    public Transform m_PointA;
    public Transform m_PointB;

    private Vector3 m_offset;

    private Interactable m_Interactable;

    void Start()
    {
        m_Interactable = GetComponent<Interactable>();
    }

    
    void Update()
    {
        if( parent != null)
        {
            transform.position = ClosestPointOnLine(parent.position) - m_offset;
        }
    }

    public void PickUp()
    {
        parent = m_Interactable.m_ActiveHand.transform;

        m_offset = parent.position - transform.position;
    }

    public void Drop()
    {
        m_Interactable.simulator.transform.position = transform.position + m_offset;

        parent = m_Interactable.simulator.transform;
    }

    Vector3 ClosestPointOnLine(Vector3 point)
    {
        Vector3 vectorA = m_PointA.position + m_offset;
        Vector3 vectorB = m_PointB.position + m_offset;

        Vector3 vVector1 = point - vectorA;
        Vector3 vVector2 = (vectorB - vectorA).normalized;

        float distance = Vector3.Dot(vVector2, vVector1);

        if (distance <= 0)
            return vectorA;

        if (distance >= Vector3.Distance(vectorA, vectorB))
            return vectorB;

        Vector3 vVector3 = vVector2 * distance;

        Vector3 vClosestPoint = vectorA + vVector3;

        return vClosestPoint;
    }
}
