using System;
using System.Collections;
using System.Collections.Generic;
using JGVR.Locomotion;
using UnityEngine;

public class LocomotionManager : MonoBehaviour
{
    public GameObject TeleportPointerPrefab;

    private GameObject _gameObjectLocomotion;

    private GameObject _teleportPointer = null;

    private int _numberOfRandomTechnique = 0;

    private int _numberOfTechniques = 0;

    private readonly List<LocomotionTechnique> _unusedLocomotionTechniques = new List<LocomotionTechnique>();

    public enum LocomotionTechnique {
        BlinkStep,
        Teleport,
        DashStep,
        Armswing,
        SmoothLocomotion,
        Cybershoes,
        None
    }

    private void Awake()
    {
        GetGameObjectsWithScripts();
    }

    private void Start()
    {
        FillListOfLocomotionTechniques();
        _numberOfTechniques = _unusedLocomotionTechniques.Count;
    }

    private void FillListOfLocomotionTechniques()
    {
        _unusedLocomotionTechniques.Add(LocomotionTechnique.Armswing);
        //_unusedLocomotionTechniques.Add(LocomotionTechnique.Cybershoes);
        _unusedLocomotionTechniques.Add(LocomotionTechnique.DashStep);
        _unusedLocomotionTechniques.Add(LocomotionTechnique.SmoothLocomotion);
        _unusedLocomotionTechniques.Add(LocomotionTechnique.Teleport);
    }

    private void GetGameObjectsWithScripts()
    {
        _gameObjectLocomotion = GameObject.Find("Locomotion");
    }

    public LocomotionTechnique GetDummyLocomotionTechnique()
    {
        TurnOffAllTechniques();
        return LocomotionTechnique.BlinkStep;

    }

    public LocomotionTechnique GetRandomLocomotionTechnique()
    {
        var random = new System.Random();
        var randomNumber = random.Next(0, _unusedLocomotionTechniques.Count);

        TurnOffAllTechniques();

        _numberOfRandomTechnique += 1;

        if (_numberOfRandomTechnique > _numberOfTechniques)
            return LocomotionTechnique.None;

        var locomotionTechnique = _unusedLocomotionTechniques[randomNumber];

        _unusedLocomotionTechniques.RemoveAt(randomNumber);

        return locomotionTechnique;
    }

    public void TurnOnLocomotionTechnique(LocomotionTechnique locomotionTechnique)
    {
        switch (locomotionTechnique)
        {
            case LocomotionTechnique.DashStep:
                TurnOnDashStep();
                break;
            case LocomotionTechnique.Armswing:
                TurnOnArmswing();
                break;
            case LocomotionTechnique.Teleport:
                TurnOnTeleport();
                break;
            case LocomotionTechnique.SmoothLocomotion:
                TurnOnSmoothLocomotion();
                break;
            case LocomotionTechnique.Cybershoes:
                TurnOnCybershoes();
                break;
            case LocomotionTechnique.BlinkStep:
                TurnOnBlinkStep();
                break;
            default:
                break;
        }
    }

    private void TurnOnTeleport()
    {
        _gameObjectLocomotion.GetComponent<Teleporter>().enabled = true;
        if (_teleportPointer != null) return;
        _teleportPointer = Instantiate(TeleportPointerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        _gameObjectLocomotion.GetComponent<Teleporter>().Pointer = _teleportPointer;
    }

    private void TurnOffTeleport()
    {
        _gameObjectLocomotion.GetComponent<Teleporter>().enabled = false;
        if (_teleportPointer == null) return;
        Destroy(_teleportPointer);
        _gameObjectLocomotion.GetComponent<Teleporter>().Pointer = null;
    }

    private void TurnOnSmoothLocomotion()
    {
        _gameObjectLocomotion.GetComponent<Smooth_locomotion>().enabled = true;
    }

    private void TurnOffSmoothLocomotion()
    {
        _gameObjectLocomotion.GetComponent<Smooth_locomotion>().enabled = false;
    }

    private void TurnOnArmswing()
    {
        _gameObjectLocomotion.GetComponent<ArmSwingLocomotionController>().enabled = true;
    }

    private void TurnOffArmswing()
    {
        _gameObjectLocomotion.GetComponent<ArmSwingLocomotionController>().enabled = false;
    }

    private void TurnOnCybershoes()
    {
        _gameObjectLocomotion.GetComponent<Cybershoes>().enabled = true;
    }

    private void TurnOffCybershoes()
    {
        _gameObjectLocomotion.GetComponent<Cybershoes>().enabled = false;
    }

    private void TurnOnDashStep()
    {
        _gameObjectLocomotion.GetComponent<DashStep>().enabled = true;
        _gameObjectLocomotion.GetComponent<DashStep>().IsEnabled = true;
    }

    private void TurnOffDashStep()
    {
        _gameObjectLocomotion.GetComponent<DashStep>().enabled = false;
        _gameObjectLocomotion.GetComponent<DashStep>().IsEnabled = false;
    }

    private void TurnOnBlinkStep()
    {
        _gameObjectLocomotion.GetComponent<BlinkStep>().enabled = true;
        _gameObjectLocomotion.GetComponent<BlinkStep>().IsEnabled = true;
    }

    private void TurnOffBlinkStep()
    {
        _gameObjectLocomotion.GetComponent<BlinkStep>().enabled = false;
        _gameObjectLocomotion.GetComponent<BlinkStep>().IsEnabled = false;
    }

    public void TurnOffAllTechniques()
    {
        TurnOffDashStep();
        TurnOffCybershoes();
        TurnOffSmoothLocomotion();
        TurnOffTeleport();
        TurnOffArmswing();
        TurnOffBlinkStep();
    }

}
