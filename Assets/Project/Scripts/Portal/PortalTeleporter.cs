using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    public Transform m_Player;
    public Transform m_Reciever;

    private bool m_PlayerIsOverlapping = false;

    void Update()
    {
        if (m_PlayerIsOverlapping)
        {
            Vector3 portalToplayer = m_Player.position - transform.position;
            float dotProduct = Vector3.Dot(transform.up, portalToplayer);

            // If this is true: the player has moved across the portal
            if(dotProduct < 0f)
            {
                // Teleport him
                float rotationDifference = -Quaternion.Angle(transform.rotation, m_Reciever.rotation);
                rotationDifference += 180;
                m_Player.Rotate(Vector3.up, rotationDifference);

                Vector3 positionOffset = Quaternion.Euler(0f, rotationDifference, 0f) * portalToplayer;
                m_Player.position = m_Reciever.position + positionOffset;

                m_PlayerIsOverlapping = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            m_PlayerIsOverlapping = true;
    }

    private void OnTriggerExit(Collider other)
    {
        m_PlayerIsOverlapping = false;
    }
}
