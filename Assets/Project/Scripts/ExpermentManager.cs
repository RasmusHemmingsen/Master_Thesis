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

    public Vector3 m_PlayerStartPosition = new Vector3(-3.5f, 0f, -3.5f);
    public Quaternion m_PlayerStartRotaion = new Quaternion(0, 0, 0, 1);

    private float m_Room1StartTime = 0f;
    private float m_Room2StartTime = 0f;
    private float m_Room3StartTime = 0f;
    
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

    public void Start()
    {
        m_CurrentLocomotionTechnique = m_LocomotionManager.GetDummyLocomotionTechnique();
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

    public void HighlightButton(GameObject buttonTop, GameObject buttonStand)
    {
        Renderer rendererTop = buttonTop.GetComponent<Renderer>();
        Renderer rendererStand = buttonStand.GetComponent<Renderer>();
        m_SwitchShader.SwitchToHighlihtButton(rendererTop, rendererStand);
    }

    public void RemoveHighligtFromButton(GameObject buttonTop, GameObject buttonStand)
    {
        Renderer rendererTop = buttonTop.GetComponent<Renderer>();
        Renderer rendererStand = buttonStand.GetComponent<Renderer>();
        m_SwitchShader.SwitchToStandardButton(rendererTop, rendererStand);
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
        m_Player.transform.position = m_PlayerStartPosition;
        m_Player.transform.rotation = m_PlayerStartRotaion;
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
        //File.WriteAllBytes(m_DirectoryPath + "/" + m_CurrentLocomotionTechnique + "Experimentroom" + numberOfExperimentRoom + "path", getPathData());

        // Write recording to file 
        File.WriteAllBytes(m_DirectoryPath + "/" + m_CurrentLocomotionTechnique + "Experimentroom" + numberOfExperimentRoom + "time", BitConverter.GetBytes(time));
    }

    private byte[] getPathData()
    {
        return System.Text.Encoding.UTF8.GetBytes("test");
    }

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
}
