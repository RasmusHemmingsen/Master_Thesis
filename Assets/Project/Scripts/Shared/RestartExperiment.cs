using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartExperiment : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ExpermentManager.m_ExpermentManager.RestartWithNewTechnique();
    }
}
