using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnloadScene : MonoBehaviour
{
    public int m_Scene;

    private bool m_Unloaded;

    private void OnTriggerEnter(Collider other)
    {
        if (m_Unloaded)
        {
            m_Unloaded = true;

            ExpermentManager.m_ExpermentManager.UnloadScene(m_Scene);
        }
    }
}
