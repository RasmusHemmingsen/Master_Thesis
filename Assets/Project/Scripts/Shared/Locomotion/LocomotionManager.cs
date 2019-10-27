﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionManager : MonoBehaviour
{
    public GameObject m_TeleportPointerPrefab;

    private GameObject m_gameObjectLocomotion;

    private GameObject m_TeleportPointer = null;

    private int m_NumberOfRandomTechnique = 0;

    private int m_NumberOfTechniques = 0;

    private List<LocomotionTechinique> m_UnusedLocomotionTechiniques = new List<LocomotionTechinique>();

    public enum LocomotionTechinique {
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
        FillListOfLocomotionTecniques();
        m_NumberOfTechniques = m_UnusedLocomotionTechiniques.Count;
    }

    private void FillListOfLocomotionTecniques()
    {
        //m_UnusedLocomotionTechiniques.Add(LocomotionTechinique.Armswing);
        //m_UnusedLocomotionTechiniques.Add(LocomotionTechinique.Cybershoes);
        m_UnusedLocomotionTechiniques.Add(LocomotionTechinique.DashStep);
        m_UnusedLocomotionTechiniques.Add(LocomotionTechinique.SmoothLocomotion);
        m_UnusedLocomotionTechiniques.Add(LocomotionTechinique.Teleport);
    }

    private void GetGameObjectsWithScripts()
    {
        m_gameObjectLocomotion = GameObject.Find("Locomotion");
    }

    public LocomotionTechinique GetDummyLocomotionTechnique()
    {
        TurnOffAllTechniques();
        TurnOnBlinkStep();
        return LocomotionTechinique.BlinkStep;

    }

    public LocomotionTechinique GetRandomLocomotionTechnique()
    {
        var random = new System.Random();
        int randomNumber = random.Next(0, m_UnusedLocomotionTechiniques.Count);

        TurnOffAllTechniques();

        m_NumberOfRandomTechnique += 1;

        if (m_NumberOfRandomTechnique > m_NumberOfTechniques)
            return LocomotionTechinique.None;

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
        m_gameObjectLocomotion.GetComponent<Teleporter>().enabled = true;
        if (m_TeleportPointer == null)
        {
            m_TeleportPointer = Instantiate(m_TeleportPointerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            m_gameObjectLocomotion.GetComponent<Teleporter>().m_pointer = m_TeleportPointer;
        }
    }

    private void TurnOffTeleport()
    {
        m_gameObjectLocomotion.GetComponent<Teleporter>().enabled = false;
        if (m_TeleportPointer != null)
        {
            Destroy(m_TeleportPointer);
            m_gameObjectLocomotion.GetComponent<Teleporter>().m_pointer = null;
        }
    }

    private void TurnOnSmoothLocomotion()
    {
        m_gameObjectLocomotion.GetComponent<Smooth_locomotion>().enabled = true;
    }

    private void TurnOffSmoothLocomotion()
    {
        m_gameObjectLocomotion.GetComponent<Smooth_locomotion>().enabled = false;
    }

    private void TurnOnArmswing()
    {
        m_gameObjectLocomotion.GetComponent<Armswing>().enabled = true;
    }

    private void TurnOffArmswing()
    {
        m_gameObjectLocomotion.GetComponent<Armswing>().enabled = false;
    }

    private void TurnOnCybershoes()
    {
        m_gameObjectLocomotion.GetComponent<Cybershoes>().enabled = true;
    }

    private void TurnOffCybershoes()
    {
        m_gameObjectLocomotion.GetComponent<Cybershoes>().enabled = false;
    }

    private void TurnOnDashStep()
    {
        //m_gameObjectLocomotion.GetComponent<DashStep>().enabled = true;
        m_gameObjectLocomotion.GetComponent<DashStep>().m_IsEnabled = true;
    }

    private void TurnOffDashStep()
    {
        //m_gameObjectLocomotion.GetComponent<DashStep>().enabled = false;
        m_gameObjectLocomotion.GetComponent<DashStep>().m_IsEnabled = false;
    }

    private void TurnOnBlinkStep()
    {
        //m_gameObjectLocomotion.GetComponent<BlinkStep>().enabled = true;
        m_gameObjectLocomotion.GetComponent<BlinkStep>().m_IsEnabled = true;
    }

    private void TurnOffBlinkStep()
    {
        //m_gameObjectLocomotion.GetComponent<BlinkStep>().enabled = false;
        m_gameObjectLocomotion.GetComponent<BlinkStep>().m_IsEnabled = false;
    }

    public void TurnOffAllTechniques()
    {
        TurnOffDashStep();
        TurnOffCybershoes();
        TurnOffSmoothLocomotion();
        TurnOffTeleport();
        //TurnOffArmswing();
        TurnOffBlinkStep();
    }

}
