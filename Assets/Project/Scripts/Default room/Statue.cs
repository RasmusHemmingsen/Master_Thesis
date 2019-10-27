using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour
{
    private bool triggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if(!triggered)
        {
            triggered = true;
            ExpermentManager.m_ExpermentManager.StartTestWithNewTechnique();
        }
    }
}
