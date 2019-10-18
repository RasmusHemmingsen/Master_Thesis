using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Room3Manager m_RoomManager;
    private void OnTriggerEnter(Collider other)
    {
        if (m_RoomManager.IsCurrentCube(other.gameObject))
        {
            m_RoomManager.CurrentCubePlacedCorrectly();
        }
    }
}
