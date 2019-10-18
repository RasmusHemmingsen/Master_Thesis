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

    public void DisableDoorhandle(GameObject gameObject)
    {
        Collider collider = gameObject.GetComponent<MeshCollider>();
        StartCoroutine(WaitForTwoSecondsAndDisableCollider(collider));
    }

    private IEnumerator WaitForTwoSecondsAndDisableCollider(Collider collider)
    {
        yield return new WaitForSeconds(2);
        collider.enabled = false;
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
