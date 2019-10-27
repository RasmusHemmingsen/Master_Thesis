using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Room3Manager RoomManager;

    private const float TableHeight = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (RoomManager.IsCurrentCube(other.gameObject) && other.transform.position.y >= TableHeight && !other.transform.parent.CompareTag("Controller"))
        {
            RoomManager.CurrentCubePlacedCorrectly();
        }
    }
}
