using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public int Scene;

    private bool _loaded;

    private void OnTriggerEnter(Collider other)
    {
        LoadNewScene();
    }

    public void LoadNewScene()
    {
        if (_loaded) return;
        SceneManager.LoadSceneAsync(Scene, LoadSceneMode.Additive);

        _loaded = true;
    }
}
