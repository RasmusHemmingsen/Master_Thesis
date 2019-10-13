using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorFix : MonoBehaviour
{
    public GameObject m_Door;
    private void Start()
    {
        m_Door.transform.rotation = new Quaternion(-90, 0, 0, 1);
    }
}
