using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Room3Manager m_RoomManager;

    private float m_TableHight = 1f;
    private void OnTriggerEnter(Collider other)
    {
        if (m_RoomManager.IsCurrentCube(other.gameObject) && other.transform.position.y >= m_TableHight)
        {
            m_RoomManager.CurrentCubePlacedCorrectly();
        }
    }
}
