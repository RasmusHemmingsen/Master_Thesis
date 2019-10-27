using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class SlidingDrawer : MonoBehaviour
{
    private Transform _parent;
    public Transform PointA;
    public Transform PointB;

    private Vector3 _offset;

    private Interactable _interactable;

    private void Start()
    {
        _interactable = GetComponent<Interactable>();
    }


    private void Update()
    {
        if( _parent != null)
        {
            transform.position = ClosestPointOnLine(_parent.position) - _offset;
        }
    }

    public void PickUp()
    {
        _parent = _interactable.ActiveHand.transform;

        _offset = _parent.position - transform.position;
    }

    public void Drop()
    {
        _interactable.Simulator.transform.position = transform.position + _offset;

        _parent = _interactable.Simulator.transform;
    }

    private Vector3 ClosestPointOnLine(Vector3 point)
    {
        var vectorA = PointA.position + _offset;
        var vectorB = PointB.position + _offset;

        var vVector1 = point - vectorA;
        var vVector2 = (vectorB - vectorA).normalized;

        var distance = Vector3.Dot(vVector2, vVector1);

        if (distance <= 0)
            return vectorA;

        if (distance >= Vector3.Distance(vectorA, vectorB))
            return vectorB;

        var vVector3 = vVector2 * distance;

        var vClosestPoint = vectorA + vVector3;

        return vClosestPoint;
    }
}
