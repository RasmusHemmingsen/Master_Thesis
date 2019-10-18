using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTriggerRoom2 : MonoBehaviour
{
    public Room2Manager m_RoomManager;

    private bool m_Triggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!m_Triggered && other.CompareTag("Player"))
        {
            m_RoomManager.Startimer();
            m_RoomManager.HighlightButton1();
            m_Triggered = true;
        }
    }
}
