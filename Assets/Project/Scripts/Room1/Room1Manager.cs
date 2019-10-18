using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room1Manager : MonoBehaviour
{
    
    private void Start()
    {
        Startimer();
    }

    public void Startimer()
    {
        ExpermentManager.m_ExpermentManager.StartTimerRoom1();
    }

    public void StopTimer()
    {
        ExpermentManager.m_ExpermentManager.StopTimerRoom1();
    }

    public void LoadScene2()
    {
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }
}
