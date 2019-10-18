﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room2Manager : MonoBehaviour
{
    public GameObject m_WallToRoom1;
    public GameObject m_WallToRoom3;
    public GameObject m_Button1;
    public GameObject m_Button2;
    
    public bool m_FirstButtonPressed = false;
    public bool m_SecondButtonPressed = false;

    private void Start()
    {
        HighlightButton1();
        StartCoroutine(Startimer());
    }
    
    private IEnumerator Startimer()
    {
        yield return new WaitForSeconds(2);
        ExpermentManager.m_ExpermentManager.StartTimerRoom2();

    }

    public void StopTimer()
    {
        ExpermentManager.m_ExpermentManager.StopTimerRoom2();
    }

    public void Button1Pressed()
    {
        RemoveHighlightFromButton1();
        CloseForRoom1();
        HighlightButton2();
        m_FirstButtonPressed = true;
    }

    public void Button2Pressed()
    {
        OpenForRoom3();
        RemoveHighlightFromButton2();
        m_SecondButtonPressed = true;
    }

    public bool BothButtonsPressed()
    {
        if (m_FirstButtonPressed && m_SecondButtonPressed)
            return true;
        return false;
    }

    public void HighlightButton1()
    {
        ExpermentManager.m_ExpermentManager.HighlightButton(m_Button1.transform.GetChild(0).gameObject, m_Button1.transform.GetChild(1).gameObject);
    }

    private void RemoveHighlightFromButton1()
    {
        ExpermentManager.m_ExpermentManager.RemoveHighligtFromButton(m_Button1.transform.GetChild(0).gameObject, m_Button1.transform.GetChild(1).gameObject);
    }

    private void HighlightButton2()
    {
        ExpermentManager.m_ExpermentManager.HighlightButton(m_Button2.transform.GetChild(0).gameObject, m_Button2.transform.GetChild(1).gameObject);
    }

    private void RemoveHighlightFromButton2()
    {
        ExpermentManager.m_ExpermentManager.RemoveHighligtFromButton(m_Button2.transform.GetChild(0).gameObject, m_Button2.transform.GetChild(1).gameObject);
    }

    private void OpenForRoom3()
    {
        if (m_FirstButtonPressed)
        {
            DisableWallToRoom3();
            LoadScene3();
        }
    }

    private void DisableWallToRoom3()
    {
        m_WallToRoom3.GetComponent<MeshRenderer>().enabled = false;
        m_WallToRoom3.GetComponent<BoxCollider>().enabled = false;

    }

    private void LoadScene3()
    {
        SceneManager.LoadSceneAsync(3, LoadSceneMode.Additive);
    }

    private void CloseForRoom1()
    {
        EnableWallToRoom1();
        UnloadScene1();
    }

    private void EnableWallToRoom1()
    {
        m_WallToRoom1.GetComponent<MeshRenderer>().enabled = true;
        m_WallToRoom1.GetComponent<BoxCollider>().enabled = true;

    }

    private void UnloadScene1()
    {
        ExpermentManager.m_ExpermentManager.UnloadScene(1);
    }
}