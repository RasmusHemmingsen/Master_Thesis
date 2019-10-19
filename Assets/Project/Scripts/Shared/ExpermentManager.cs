using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using System.Globalization;

public class ExpermentManager : MonoBehaviour
{
    public static ExpermentManager m_ExpermentManager;
    public GameObject m_Player;

    public Vector3 m_PlayerStartPosition = new Vector3(-3.5f, 0f, -3.5f);
    public Quaternion m_PlayerStartRotaion = new Quaternion(0, 0, 0, 1);

    private float m_Room1StartTime = 0f;
    private float m_Room2StartTime = 0f;
    private float m_Room3StartTime = 0f;
    
    private float m_Room1Time;
    private float m_Room2Time;
    private float m_Room3Time;

    private string m_DirectoryPath = "ExperimentResults";
    private string m_Filename;
    
    private LocomotionManager m_LocomotionManager;
    private LocomotionManager.LocomotionTechinique m_CurrentLocomotionTechnique;
    private SwitchShader m_SwitchShader;

    private bool m_GameStart;

    private void Awake()
    {
        if(!m_GameStart)
        {
            m_ExpermentManager = this;

            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

            m_LocomotionManager = FindObjectOfType<LocomotionManager>();
            m_SwitchShader = FindObjectOfType<SwitchShader>();

            m_GameStart = true;

            SetPlayerToStartPosition();
        }      
    }

    public void Start()
    {
        m_CurrentLocomotionTechnique = m_LocomotionManager.GetDummyLocomotionTechnique();
        if (m_Filename == null)
            m_Filename = "Experiment" + System.DateTime.Now.ToString("s", CultureInfo.CreateSpecificCulture("en-US"));
    }

    #region Highhight
    public void HighlightCube(GameObject cube)
    {
        Renderer renderer = cube.GetComponent<Renderer>();
        m_SwitchShader.SwitchToHighlightCube(renderer);
    }
    public void HighlightButton(GameObject buttonTop, GameObject buttonStand)
    {
        Renderer rendererTop = buttonTop.GetComponent<Renderer>();
        Renderer rendererStand = buttonStand.GetComponent<Renderer>();
        m_SwitchShader.SwitchToHighlihtButton(rendererTop, rendererStand);
    }
    public void HighlightHandle(GameObject handle)
    {
        Renderer renderer = handle.GetComponent<Renderer>();
        m_SwitchShader.SwitchToHighlightHandle(renderer);
    }

    public void HighlightTable(GameObject table)
    {
        Renderer renderer = table.GetComponent<Renderer>();
    }
    #endregion

    #region Remove Highlight

    public void RemoveHighligtFromCube(GameObject gameObject)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        m_SwitchShader.SwitchToStandardCube(renderer);
    }

    public void RemoveHighligtFromButton(GameObject buttonTop, GameObject buttonStand)
    {
        Renderer rendererTop = buttonTop.GetComponent<Renderer>();
        Renderer rendererStand = buttonStand.GetComponent<Renderer>();
        m_SwitchShader.SwitchToStandardButton(rendererTop, rendererStand);
    }

    public void RemoveHighlightFromHandle(GameObject handle)
    {
        Renderer renderer = handle.GetComponent<Renderer>();
        m_SwitchShader.SwitchToStandardHandle(renderer);
    }

    public void RemoveHighlightFromTable(GameObject table)
    {
        Renderer renderer = table.GetComponent<Renderer>();
    }
    #endregion

    public void UnloadScene(int scene)
    {
        StartCoroutine(Unload(scene));
    }

    private IEnumerator Unload(int scene)
    {
        yield return null;

        SceneManager.UnloadSceneAsync(scene);
    }

    public void RestartWithNewTechnique()
    {
        WriteTechniqueResults();
        m_CurrentLocomotionTechnique = m_LocomotionManager.GetRandomLocomotionTechnique();
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        SetPlayerToStartPosition();
        UnloadScene(3);
    }

    private void SetPlayerToStartPosition()
    {
        m_Player.transform.position = m_PlayerStartPosition;
        m_Player.transform.rotation = m_PlayerStartRotaion;
    }

    private void WriteTechniqueResults()
    {
        string data = PepareTimeDataForWiriteTofile();
        WriteDataToFile(data);
    }

    private string PepareTimeDataForWiriteTofile()
    {
        string data = m_CurrentLocomotionTechnique + "\n" +
            "Room 1 Time: " + m_Room1Time + "\n" +
            "Room 2 Time: " + m_Room2Time + "\n" +
            "Room 3 Time: " + m_Room3Time + "\n\n";
        return data;
    }

    private void WriteDataToFile(string data)
    {
        if (!System.IO.Directory.Exists(m_DirectoryPath))
        {
            System.IO.Directory.CreateDirectory(m_DirectoryPath);
        }

        StreamWriter writer = System.IO.File.AppendText(m_DirectoryPath + "/" + m_Filename);
        writer.Write(data);

        // Write user path to file
    }

    private byte[] getPathData()
    {
        return System.Text.Encoding.UTF8.GetBytes("test");
    }

    #region Timers
    public void StartTimerRoom1()
    {
        if(m_Room1StartTime == 0f)
            m_Room1StartTime = Time.time;
    }

    public void StartTimerRoom2()
    {
        if (m_Room2StartTime == 0f)
            m_Room2StartTime = Time.time;
    }

    public void StartTimerRoom3()
    {
        if (m_Room3StartTime == 0f)
            m_Room3StartTime = Time.time;
    }

    public void StopTimerRoom1()
    {
        m_Room1Time = Time.time - m_Room1StartTime;
        print(m_Room1Time);
    }

    public void StopTimerRoom2()
    {
        m_Room2Time = Time.time - m_Room2StartTime;
        print(m_Room2Time);
    }

    public void StopTimerRoom3()
    {
        m_Room3Time = Time.time - m_Room3StartTime;
    }
    #endregion
}
