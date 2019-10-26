using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room2Manager : MonoBehaviour
{
    public GameObject m_WallToRoom1;
    public GameObject m_WallToRoom3;
    public GameObject m_Button1;
    public GameObject m_Button2;
    
    private bool m_FirstButtonPressed = false;
    private bool m_SecondButtonPressed = false;

    private void Start()
    {
        HighlightButton1();
        StartCoroutine(Startimer());
    }
    
    private IEnumerator Startimer()
    {
        yield return new WaitForSeconds(1);
        TimeManager.m_TimeManager.StartTimerRoom2();
        DistanceManager.m_DistanceManager.SetActiveRoomForDistance(2);

    }

    private void StopTimerBotton(int button)
    {
        TimeManager.m_TimeManager.StopTimerRoom2Button(button);
    }

    public void Button1Pressed()
    {
        if (!m_FirstButtonPressed)
        {
            RemoveHighlightFromButton1();
            CloseForRoom1();
            HighlightButton2();
            StopTimerBotton(1);
            m_FirstButtonPressed = true;
        }
    }

    public void Button2Pressed()
    {
        if (!m_SecondButtonPressed && m_FirstButtonPressed)
        {
            OpenForRoom3();
            StopTimerBotton(2);
            RemoveHighlightFromButton2();
            m_SecondButtonPressed = true;
        }
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
