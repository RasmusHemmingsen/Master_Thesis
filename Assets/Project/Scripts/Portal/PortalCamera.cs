using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public Transform m_PlayerCamera;
    public Transform m_portal;
    public Transform m_OtherPotal;


    // Update is called once per frame
    void Update()
    {
        Vector3 playerOffsetFromPortal = m_PlayerCamera.position - m_OtherPotal.position;
        transform.position = m_portal.position + playerOffsetFromPortal;

        float angularDifferencBetweenPortalsRotations = Quaternion.Angle(m_portal.rotation, m_OtherPotal.rotation);

        Quaternion portalRotationDifference = Quaternion.AngleAxis(angularDifferencBetweenPortalsRotations, Vector3.up);
        Vector3 newCameraDirection = portalRotationDifference * m_PlayerCamera.forward;
        transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
    }
}
