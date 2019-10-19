using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room3Manager : MonoBehaviour
{
    public List<GameObject> m_ListOfCubes;
    public GameObject m_Handle;

    public GameObject m_CurrentCube;
    public GameObject m_NextCube;

    private int m_NumberOfCubesCurrectPlaced = 0;
    private int m_CubeListSize;

    void Start()
    {
        HighlightHandle();
        m_CurrentCube = m_ListOfCubes[m_NumberOfCubesCurrectPlaced];
        m_NextCube = GetNextCubeIfThereIsOne();
        m_CubeListSize = m_ListOfCubes.Count;
    }

    public void Startimer()
    {
        ExpermentManager.m_ExpermentManager.StartTimerRoom3();
    }

    public void StopTimerForRoom2()
    {
        ExpermentManager.m_ExpermentManager.StopTimerRoom2();
    }

    #region Highlighting
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
    #endregion

    public void CurrentCubePlacedCorrectly()
    {
        if(m_NumberOfCubesCurrectPlaced == 0)
        {
            m_Handle.GetComponent<Door>().StartCloseDoorAnimation();
            ExpermentManager.m_ExpermentManager.UnloadScene(2);
        }

        RemoveHighlightFromCurrentCube();
        m_NumberOfCubesCurrectPlaced += 1;
        if (m_NumberOfCubesCurrectPlaced >= m_CubeListSize)
        {
            ExpermentManager.m_ExpermentManager.StopTimerRoom3();
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
