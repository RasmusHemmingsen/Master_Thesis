﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room3Manager : MonoBehaviour
{
    public List<GameObject> m_ListOfCubes;
    public GameObject m_Handle;

    private GameObject m_CurrentCube;
    private GameObject m_NextCube;

    private int m_NumberOfCubesCurrectPlaced = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        HighlightHandle();
        m_CurrentCube = m_ListOfCubes[m_NumberOfCubesCurrectPlaced];
        m_NextCube = m_ListOfCubes[m_NumberOfCubesCurrectPlaced + 1];
    }

    private void HighlightHandle()
    {
        ExpermentManager.m_ExpermentManager.HighlightHandle(m_Handle);
    }

    public void RemoveHighlightFromHandle()
    {
        ExpermentManager.m_ExpermentManager.RemoveHighlightFromHandle(m_Handle);
    }

    public void HighlightCurrentCube()
    {
        ExpermentManager.m_ExpermentManager.HighlightCube(m_CurrentCube);
    }

    private void RemoveHighlightFromCurrentCube()
    {
        ExpermentManager.m_ExpermentManager.RemoveHighligtFromCube(m_CurrentCube);
    }

    public void CurrentCubePlacedCorrectly()
    {
        RemoveHighlightFromCurrentCube();
        m_NumberOfCubesCurrectPlaced += 1;
        if (m_NumberOfCubesCurrectPlaced <= m_ListOfCubes.Count)
        {
            m_CurrentCube = m_NextCube;
            m_NextCube = m_ListOfCubes[m_NumberOfCubesCurrectPlaced + 1];
            HighlightCurrentCube();
        }
        else
        {
            ExpermentManager.m_ExpermentManager.RestartWithNewTechnique();
        }
    }

    public bool IsCurrentCube(GameObject gameObject)
    {
        if (GameObject.ReferenceEquals(gameObject, m_CurrentCube))
            return true;
        return false;
    }
}