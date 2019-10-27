using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnloadScene : MonoBehaviour
{
    public int Scene;

    private bool _unloaded;

    private void OnTriggerEnter(Collider other)
    {
        if (!_unloaded) return;
        _unloaded = true;

        ExperimentManager.ExperimentManagerVariable.UnloadScene(Scene);
    }
}
