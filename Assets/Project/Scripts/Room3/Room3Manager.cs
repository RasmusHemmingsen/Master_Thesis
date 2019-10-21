using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room3Manager : MonoBehaviour
{
    public List<GameObject> m_ListOfCubes;
    public GameObject m_Handle;
    public GameObject m_table;

    private GameObject m_CurrentCube;
    private GameObject m_NextCube;

    private int m_NumberOfCubesCurrectPlaced = 0;
    private int m_CubeListSize;

    void Start()
    {
        m_CubeListSize = m_ListOfCubes.Count;
        HighlightHandle();
        m_CurrentCube = m_ListOfCubes[m_NumberOfCubesCurrectPlaced];
        m_NextCube = GetNextCubeIfThereIsOne();   
    }

    public void Startimer()
    {
        TimeManager.m_TimeManager.StartTimerRoom3();
    }

    private void HighlightHandle()
    {
        ExpermentManager.m_ExpermentManager.HighlightHandle(m_Handle);
    }

    public void HandlePressed()
    {
        ExpermentManager.m_ExpermentManager.RemoveHighlightFromHandle(m_Handle);
        TimeManager.m_TimeManager.StopTimerRoom2();
    }

    public void HighlightCurrentCube()
    {
        ExpermentManager.m_ExpermentManager.HighlightCube(m_CurrentCube);
    }

    private void RemoveHighlightFromCurrentCubeAndTable()
    {
        ExpermentManager.m_ExpermentManager.RemoveHighligtFromCube(m_CurrentCube);
        ExpermentManager.m_ExpermentManager.RemoveHighlightFromTable(m_table);
    }

    public void HighlightTable()
    {
        ExpermentManager.m_ExpermentManager.HighlightTable(m_table);
    }

    public void CurrentCubePlacedCorrectly()
    {
        if(m_NumberOfCubesCurrectPlaced == 0)
        {
            m_Handle.GetComponent<Door>().StartCloseDoorAnimation();
            ExpermentManager.m_ExpermentManager.UnloadScene(2);
        }

        RemoveHighlightFromCurrentCubeAndTable();
        m_NumberOfCubesCurrectPlaced += 1;      

        TimeManager.m_TimeManager.StopTimerRoom3Cube(m_NumberOfCubesCurrectPlaced, false);

        if (m_NumberOfCubesCurrectPlaced >= m_CubeListSize)
        {
            ExpermentManager.m_ExpermentManager.RestartWithNewTechnique(); 
        }

        if(m_NextCube != null)
        {
            m_CurrentCube = m_NextCube;
            m_NextCube = GetNextCubeIfThereIsOne();
        }

        HighlightCurrentCube();
    }

    private GameObject GetNextCubeIfThereIsOne()
    {
        if (m_NumberOfCubesCurrectPlaced + 1 >= m_CubeListSize)
            return null;
        return m_ListOfCubes[m_NumberOfCubesCurrectPlaced + 1];
    }

    public bool IsCurrentCube(GameObject gameObject)
    {
        if (GameObject.ReferenceEquals(gameObject, m_CurrentCube))
            return true;
        return false;
    }
}
