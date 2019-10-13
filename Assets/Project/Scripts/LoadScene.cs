using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public int m_Scene;

    private bool m_Loaded;

    private void OnTriggerEnter(Collider other)
    {
        LoadNewScene();
    }

    public void LoadNewScene()
    {
        if (!m_Loaded)
        {
            SceneManager.LoadSceneAsync(m_Scene, LoadSceneMode.Additive);

            m_Loaded = true;
        }
    }
}
