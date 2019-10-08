using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionManager : MonoBehaviour
{
    public GameObject m_TeleportPointerPrefab;

    private GameObject m_gameObjectTeleport;
    private GameObject m_gameObjectSmoothLocomotion;
    private GameObject m_gameObjectCybershoes;
    private GameObject m_gameObjectArmswing;
    private GameObject m_gameObjectBlinkStep;
    private GameObject m_gameObjectDashStep;

    private List<LocomotionTechinique> m_UnusedLocomotionTechiniques = new List<LocomotionTechinique>();

    public enum LocomotionTechinique {
        BlinkStep,
        Teleport,
        DashStep,
        Armswing,
        SmoothLocomotion,
        Cybershoes
    }

    private void Awake()
    {
        GetGameObjectsWithScripts();
        FillListOfLocomotionTecniques();
    }

    private void FillListOfLocomotionTecniques()
    {
        m_UnusedLocomotionTechiniques.Add(LocomotionTechinique.Armswing);
        m_UnusedLocomotionTechiniques.Add(LocomotionTechinique.Cybershoes);
        m_UnusedLocomotionTechiniques.Add(LocomotionTechinique.DashStep);
        m_UnusedLocomotionTechiniques.Add(LocomotionTechinique.SmoothLocomotion);
        m_UnusedLocomotionTechiniques.Add(LocomotionTechinique.Teleport);
    }

    private void GetGameObjectsWithScripts()
    {
        m_gameObjectSmoothLocomotion = GameObject.Find("Player");
        m_gameObjectTeleport = GameObject.Find("Controller (right)");
        //m_gameObjectArmswing = GameObject.Find("");
        //m_gameObjectBlinkStep = GameObject.Find("");
        //m_gameObjectDashStep = GameObject.Find("");
        //m_gameObjectCybershoes = GameObject.Find("");
    }

    public LocomotionTechinique GetDummyLocomotionTechnique()
    {
        TurnOffAllTechniques();
        TurnOnBlinkStep();
        return LocomotionTechinique.BlinkStep;
    }

    public LocomotionTechinique GetRandomLocomotionTechnique()
    {
        int randomNumber = UnityEngine.Random.Range(0, m_UnusedLocomotionTechiniques.Count + 1);

        TurnOffAllTechniques();

        LocomotionTechinique locomotionTechinique = m_UnusedLocomotionTechiniques[randomNumber];

        switch (locomotionTechinique)
        {
            case LocomotionTechinique.DashStep:
                TurnOnDashStep();
                break;
            case LocomotionTechinique.Armswing:
                TurnOnArmswing();
                break;
            case LocomotionTechinique.Teleport:
                TurnOnTeleport();
                break;
            case LocomotionTechinique.SmoothLocomotion:
                TurnOnSmoothLocomotion();
                break;
            case LocomotionTechinique.Cybershoes:
                TurnOnCybershoes();
                break;
        }

        m_UnusedLocomotionTechiniques.RemoveAt(randomNumber);

        return locomotionTechinique;
    }

    private void TurnOnTeleport()
    {
        m_gameObjectTeleport.GetComponent<Teleporter>().enabled = true;
        if (m_TeleportPointerPrefab == null)
            Instantiate(m_TeleportPointerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    private void TurnOffTeleport()
    {
        m_gameObjectTeleport.GetComponent<Teleporter>().enabled = false;
        if (m_TeleportPointerPrefab != null)
            Destroy(m_TeleportPointerPrefab);
    }

    private void TurnOnSmoothLocomotion()
    {
        m_gameObjectSmoothLocomotion.GetComponent<Smooth_locomotion>().enabled = true;
    }

    private void TurnOffSmoothLocomotion()
    {
        m_gameObjectSmoothLocomotion.GetComponent<Smooth_locomotion>().enabled = false;
    }

    private void TurnOnArmswing()
    {
        //m_gameObjectArmswing.GetComponent<>().enabled = true;
    }

    private void TurnOffArmswing()
    {
        //m_gameObjectArmswing.GetComponent<>().enabled = false;
    }

    private void TurnOnCybershoes()
    {
        //m_gameObjectCybershoes.GetComponent<>().enabled = true;
    }

    private void TurnOffCybershoes()
    {
        //m_gameObjectCybershoes.GetComponent<>().enabled = false;
    }

    private void TurnOnDashStep()
    {
        //m_gameObjectDashStep.GetComponent<>().enabled = true;
    }

    private void TurnOffDashStep()
    {
        //m_gameObjectDashStep.GetComponent<>().enabled = false;
    }

    private void TurnOnBlinkStep()
    {
        //m_gameObjectBlinkStep.GetComponent<>().enabled = true;
    }

    private void TurnOffBlinkStep()
    {
        //m_gameObjectBlinkStep.GetComponent<>().enabled = false;
    }

    private void TurnOffAllTechniques()
    {
        TurnOffDashStep();
        TurnOffCybershoes();
        TurnOffSmoothLocomotion();
        TurnOffTeleport();
        TurnOffArmswing();
        TurnOffBlinkStep();
    }

}
