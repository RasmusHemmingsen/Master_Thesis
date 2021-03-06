﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room1Manager : MonoBehaviour
{
    private bool _handlePressed = false;

    [SerializeField]
    private DescriptionManager _descriptionManager;

    private void Start()
    {
        StartTimer();
        _descriptionManager.SwitchDescription(ExperimentManager.ExperimentManagerVariable.CurrentLocomotionTechnique, false);
    }

    public void HandleGrapped()
    {
        if (_handlePressed)
            return;
        _handlePressed = true;
        LoadScene2();
        StopTimer();
    }

    public void HandleDropped(GameObject handle)
    {
        DisableDoorHandle(handle);
    }


    private void DisableDoorHandle(GameObject handle)
    {
        var handleCollider = handle.GetComponent<MeshCollider>();
        StartCoroutine(DisableCollider(handleCollider));
    }

    private IEnumerator DisableCollider(Collider col)
    {
        yield return new WaitForSeconds(1f);
        col.enabled = false;
    }

    private void StartTimer()
    {
        TimeManager.TimeManagerVariable.StartTimerRoom1();
    }

    private void StopTimer()
    {
        TimeManager.TimeManagerVariable.StopTimerRoom1();
    }

    public void LoadScene2()
    {
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }
}
