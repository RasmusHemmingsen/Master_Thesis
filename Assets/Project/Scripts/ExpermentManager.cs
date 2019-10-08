using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class ExpermentManager : MonoBehaviour
{
    public static ExpermentManager m_ExpermentManager;

    public int NumberOfExperimentRoom = 1;
    public int NumberOfExperimentRun = 0;

    private string m_DirectoryPath = "C:/ExperimentResults";
    
    private LocomotionManager m_LocomotionManager;
    private LocomotionManager.LocomotionTechinique m_CurrentLocomotionTechnique;

    private void Awake()
    {
        if (m_ExpermentManager == null)
        {
            m_ExpermentManager = this;
        }
        else if (m_ExpermentManager != this)
        {
            m_ExpermentManager.m_LocomotionManager = FindObjectOfType<LocomotionManager>();
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        m_LocomotionManager = FindObjectOfType<LocomotionManager>();
    }

    private void Start()
    {
        m_CurrentLocomotionTechnique = m_LocomotionManager.GetDummyLocomotionTechnique();
    }

    public void NextLevel()
    {
        WriteDataToFile(NumberOfExperimentRoom);

        if (NumberOfExperimentRoom < 3)
            NumberOfExperimentRoom++;
        else
        {
            NumberOfExperimentRun++;

            if (NumberOfExperimentRun >= 6)
                Application.Quit();

            NumberOfExperimentRoom = 1;

            m_LocomotionManager.GetRandomLocomotionTechnique();
        }

        SceneManager.LoadScene("Experiment_room_" + NumberOfExperimentRoom);
    }

    private void WriteDataToFile(int numberOfExperimentRoom)
    {
        if (!Directory.Exists(m_DirectoryPath))
        {

            Directory.CreateDirectory(m_DirectoryPath);
        }

        // Write user path to file
        File.WriteAllBytes(m_DirectoryPath + "/" + m_CurrentLocomotionTechnique + "Experimentroom" + numberOfExperimentRoom, getPathData());

        // Write recording to file 
    }

    private byte[] getPathData()
    {
        return System.Text.Encoding.UTF8.GetBytes("test");
    }
}
