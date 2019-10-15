using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class ExpermentManager : MonoBehaviour
{
    public static ExpermentManager m_ExpermentManager;
    public GameObject m_Player;

    private int NumberOfExperimentRun = 0;

    private float m_Room1StartTime;
    private float m_Room2StartTime;
    private float m_Room3StartTime;
    
    private float m_Room1Time;
    private float m_Room2Time;
    private float m_Room3Time;

    private string m_DirectoryPath = "C:/ExperimentResults";
    
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

    public void HighlightCube(GameObject gameObject)
    {
        Renderer renderer =  gameObject.GetComponent<Renderer>();
        m_SwitchShader.SwitchToHighlightCube(renderer);
    }

    public void RemoveHighligtFromCube(GameObject gameObject)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        m_SwitchShader.SwitchToStandardCube(renderer);
    }

    public void Start()
    {
        m_CurrentLocomotionTechnique = m_LocomotionManager.GetDummyLocomotionTechnique();
    }

    public void UnloadScene(int scene)
    {
        StartCoroutine(Unload(scene));
    }

    IEnumerator Unload(int scene)
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
        m_Player.transform.position = new Vector3(-3.5f, 0f, -4f);
        m_Player.transform.rotation = new Quaternion(0, 0, 0, 1);
    }

    private void WriteTechniqueResults()
    {
        WriteDataToFile(1, m_Room1Time);
        WriteDataToFile(2, m_Room2Time);
        WriteDataToFile(3, m_Room3Time);
    }

    private void WriteDataToFile(int numberOfExperimentRoom, float time)
    {
        if (!Directory.Exists(m_DirectoryPath))
        {

            Directory.CreateDirectory(m_DirectoryPath);
        }

        // Write user path to file
        File.WriteAllBytes(m_DirectoryPath + "/" + m_CurrentLocomotionTechnique + "Experimentroom" + numberOfExperimentRoom + "path", getPathData());

        // Write recording to file 
        File.WriteAllBytes(m_DirectoryPath + "/" + m_CurrentLocomotionTechnique + "Experimentroom" + numberOfExperimentRoom + "time", BitConverter.GetBytes(time));
    }

    private byte[] getPathData()
    {
        return System.Text.Encoding.UTF8.GetBytes("test");
    }

    public void StartTimerRoom1()
    {
        m_Room1StartTime = Time.time;
    }

    public void StartTimerRoom2()
    {
        m_Room2StartTime = Time.time;
    }

    public void StartTimerRoom3()
    {
        m_Room3StartTime = Time.time;
    }

    public void StopTimerRoom1()
    {
        m_Room1Time = Time.time - m_Room1StartTime;
    }

    public void StopTimerRoom2()
    {
        m_Room2Time = Time.time - m_Room2StartTime;
    }

    public void StopTimerRoom3()
    {
        m_Room3Time = Time.time - m_Room3StartTime;
    }
}
