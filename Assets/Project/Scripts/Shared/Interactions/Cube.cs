using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Cube : MonoBehaviour
{
    public Room3Manager m_RoomManager;
    private Interactable m_Interactable;

    private void Start()
    {
        m_Interactable = gameObject.GetComponent<Interactable>();
    }

    public void PickUp(int cube)
    {
        if (m_RoomManager.IsCurrentCube(gameObject))
        {
            TimeManager.m_TimeManager.StopTimerRoom3Cube(cube, true);
            m_RoomManager.HighlightTable();
        }

        m_Interactable.DefaultPickUp();
    }
}
