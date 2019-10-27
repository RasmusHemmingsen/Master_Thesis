using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room1Manager : MonoBehaviour
{
    private bool m_HandlePressed = false;

    private DescriptionManager m_DescriptionManager;

    private void Awake()
    {

        m_DescriptionManager = FindObjectOfType<DescriptionManager>();
    }

    private void Start()
    {
        Startimer();
        m_DescriptionManager.SwitchDescription(ExpermentManager.m_ExpermentManager.m_CurrentLocomotionTechnique);
    }

    public void HandleGrabed()
    {
        if (m_HandlePressed)
            return;
        m_HandlePressed = true;
        LoadScene2();
        StopTimer();
    }

    public void HandleDroped(GameObject gameObject)
    {
        DisableDoorhandle(gameObject);
    }


    private void DisableDoorhandle(GameObject gameObject)
    {
        Collider collider = gameObject.GetComponent<MeshCollider>();
        StartCoroutine(DisableCollider(collider));
    }

    private IEnumerator DisableCollider(Collider collider)
    {
        yield return new WaitForSeconds(1f);
        collider.enabled = false;
    }

    private void Startimer()
    {
        TimeManager.m_TimeManager.StartTimerRoom1();
    }

    private void StopTimer()
    {
        TimeManager.m_TimeManager.StopTimerRoom1();
    }

    public void LoadScene2()
    {
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }
}
