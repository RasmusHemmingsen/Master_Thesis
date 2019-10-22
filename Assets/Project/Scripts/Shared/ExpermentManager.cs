using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExpermentManager : MonoBehaviour
{
    public static ExpermentManager m_ExpermentManager;
    public GameObject m_Player;

    public Vector3 m_PlayerStartPosition = new Vector3(-3.5f, 0f, -3.5f);
    public Quaternion m_PlayerStartRotaion = new Quaternion(0, 0, 0, 1);
    
    private LocomotionManager m_LocomotionManager;
    private LocomotionManager.LocomotionTechinique m_CurrentLocomotionTechnique;
    private SwitchShader m_SwitchShader;
    private WriteToFile m_WriteToFile;

    private bool m_GameStart;

    private void Awake()
    {
        if(!m_GameStart)
        {
            m_ExpermentManager = this;

            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

            m_LocomotionManager = FindObjectOfType<LocomotionManager>();
            m_SwitchShader = FindObjectOfType<SwitchShader>();
            m_WriteToFile = gameObject.AddComponent<WriteToFile>();         

            m_GameStart = true;

            SetPlayerToStartPosition();
        }      
    }

    public void Start()
    {
        m_CurrentLocomotionTechnique = m_LocomotionManager.GetDummyLocomotionTechnique();
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
        m_WriteToFile.WriteAllDataToFile(m_CurrentLocomotionTechnique);
        m_CurrentLocomotionTechnique = m_LocomotionManager.GetRandomLocomotionTechnique();

        if (m_CurrentLocomotionTechnique == LocomotionManager.LocomotionTechinique.None)
            Application.Quit();
        StartCoroutine(RestartScenes());
    }

    private IEnumerator RestartScenes()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        yield return new WaitForSeconds(2);
        SetPlayerToStartPosition();
        UnloadScene(3);
    }

    private void SetPlayerToStartPosition()
    {
        m_Player.transform.position = m_PlayerStartPosition;
        m_Player.transform.rotation = m_PlayerStartRotaion;
    }
}
