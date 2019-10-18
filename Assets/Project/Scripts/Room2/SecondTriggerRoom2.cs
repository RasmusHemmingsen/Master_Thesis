using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondTriggerRoom2 : MonoBehaviour
{
    public Room2Manager m_RoomManager;

    private bool m_Triggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!m_Triggered && other.CompareTag("Controller"))
        {
            m_RoomManager.StopTimer();
            m_Triggered = true;
        }
    }
}
