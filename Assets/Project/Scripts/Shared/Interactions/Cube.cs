using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Cube : MonoBehaviour
{
    public Room3Manager RoomManager;
    private Interactable _interactable;

    private void Start()
    {
        _interactable = gameObject.GetComponent<Interactable>();
    }

    public void PickUp(int cube)
    {
        if (RoomManager.IsCurrentCube(gameObject))
        {
            TimeManager.TimeManagerVariable.StopTimerRoom3Cube(cube, true);
            RoomManager.HighlightTable();
        }

        _interactable.DefaultPickUp();
    }
}
