using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Interactable))]
public class ButtonClick : MonoBehaviour
{
    Transform parent;
    public Transform m_PointA;
    public Transform m_PointB;

    private Vector3 m_offset;

    private Interactable m_Interactable;

    public UnityEvent m_HitPointA;
    public UnityEvent m_HitPointB;
    public UnityEvent m_ReleasedA;
    public UnityEvent m_ReleasedB;


    private int m_State = 0;
    private int m_PrevState = 0;

    void Start()
    {
        m_Interactable = GetComponent<Interactable>();
    }


    void Update()
    {
        if (parent != null)
        {
            transform.position = ClosestPointOnLine(parent.position) - m_offset;
        }
        else
        {
            transform.position = ClosestPointOnLine(transform.position);
        }

        if (transform.position == m_PointA.position)
            m_State = 1;
        else if (transform.position == m_PointB.position)
            m_State = 2;
        else
            m_State = 0;

        if (m_State == 1 && m_PrevState == 0)
            m_HitPointA.Invoke();
        else if (m_State == 2 && m_PrevState == 0)
            m_HitPointB.Invoke();
        else if (m_State == 0 && m_PrevState == 1)
            m_ReleasedA.Invoke();
        else if (m_State == 0 && m_PrevState == 2)
            m_ReleasedB.Invoke();


        m_PrevState = m_State;
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
