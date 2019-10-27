using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class ExpermentManager : MonoBehaviour
{
    public static ExpermentManager m_ExpermentManager;
    public GameObject m_Player;

    public Vector3 m_PlayerStartTestPosition = new Vector3(-3.5f, 1f, -3f);
    public Quaternion m_PlayerStartTestRotaion = new Quaternion(0, -90, 0, 1);

    public Vector3 m_PlayerStartDefaultPosition = new Vector3(-11.5f, 0f, -12f);
    public Quaternion m_PlayerStartDefaultRotaion = new Quaternion(0, 0, 0, 1);

    private LocomotionManager m_LocomotionManager;
    [HideInInspector]
    public LocomotionManager.LocomotionTechinique m_CurrentLocomotionTechnique;
    private SwitchShader m_SwitchShader;
    private WriteToFile m_WriteToFile;

    private float m_FadeTime = 0.5f;

    private Vector3 m_PlayerResetRoom1 = new Vector3(0, 0, 0);
    private Vector3 m_PlayerResetRoom2 = new Vector3(20.8f, 0, 31.2f);
    private Vector3 m_PlayerResetRoom3 = new Vector3(41, 0, 60.4f);

    private bool m_GameStart;
    private bool m_First = true;

    private void Awake()
    {
        if(!m_GameStart)
        {
            m_ExpermentManager = this;

            m_LocomotionManager = FindObjectOfType<LocomotionManager>();
            m_SwitchShader = FindObjectOfType<SwitchShader>();
            m_WriteToFile = gameObject.AddComponent<WriteToFile>();         

            m_GameStart = true;

        }      
    }

    private void Start()
    {
        GotoDefaultroom();
    }

    private void Update()
    {
        CheckForResetPlayer();
    }

    private void CheckForResetPlayer()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            switch (DistanceManager.m_DistanceManager.GetActiveRoom())
            {
                case 3:
                    StartCoroutine(ResetPlayer(m_PlayerResetRoom3));
                    break;
                case 2:
                    StartCoroutine(ResetPlayer(m_PlayerResetRoom2));
                    break;
                case 1:
                    StartCoroutine(ResetPlayer(m_PlayerResetRoom1));
                    break;
                case 0:
                    StartCoroutine(ResetPlayer(m_PlayerStartDefaultPosition));
                    break;
            }
        }
    }

    private IEnumerator ResetPlayer(Vector3 position)
    {
        SteamVR_Fade.Start(Color.black, m_FadeTime, true);

        // Apply translation 
        yield return new WaitForSeconds(m_FadeTime);
        m_Player.transform.position = position;

        // Fade to clear
        SteamVR_Fade.Start(Color.clear, m_FadeTime, true);
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
        m_SwitchShader.SwitchToHighlightTable(renderer);
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
        m_SwitchShader.SwitchToStandardTable(renderer);
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

    public void GotoDefaultroom()
    {
        DistanceManager.m_DistanceManager.SetActiveRoomForDistance(0);
        SceneManager.LoadSceneAsync(4, LoadSceneMode.Additive);
        m_LocomotionManager.TurnOffAllTechniques();
        if (!m_First)
        {
            m_WriteToFile.WriteAllDataToFile(m_CurrentLocomotionTechnique);
            UnloadScene(3);
        }
        SetPlayerToDefaultStartPosition();
    }

    public void StartTestWithNewTechnique()
    {
        DistanceManager.m_DistanceManager.ResetDistanceData();
        if (m_First)
        {
            m_CurrentLocomotionTechnique = m_LocomotionManager.GetDummyLocomotionTechnique();
            m_First = false;
        }
        else
            m_CurrentLocomotionTechnique = m_LocomotionManager.GetRandomLocomotionTechnique();

        if (m_CurrentLocomotionTechnique == LocomotionManager.LocomotionTechinique.None)
            Application.Quit();
        StartCoroutine(StartTest());
    }

    private IEnumerator StartTest()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        yield return new WaitForSeconds(0.5f);
        SetPlayerToTestStartPosition();
        DistanceManager.m_DistanceManager.SetActiveRoomForDistance(1);
        Unload(4);
    }

    private void SetPlayerToTestStartPosition()
    {
        m_Player.transform.position = m_PlayerStartTestPosition;
        m_Player.transform.rotation = m_PlayerStartTestRotaion;
    }

    private void SetPlayerToDefaultStartPosition()
    {
        m_Player.transform.position = m_PlayerStartDefaultPosition;
        m_Player.transform.rotation = m_PlayerStartDefaultRotaion;
    }
}
