using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class ExperimentManager : MonoBehaviour
{
    public static ExperimentManager ExperimentManagerVariable;
    public GameObject Player;

    public SteamVR_ActionSet ActionSet;

    public Vector3 PlayerStartTestPosition = new Vector3(-3.5f, 1f, -3f);
    public Quaternion PlayerStartTestRotation = new Quaternion(0, -90, 0, 1);

    public Vector3 PlayerStartDefaultPosition = new Vector3(-11.5f, 0f, -12f);
    public Quaternion PlayerStartDefaultRotation = new Quaternion(0, 0, 0, 1);

    private LocomotionManager _locomotionManager;
    [HideInInspector]
    public LocomotionManager.LocomotionTechnique CurrentLocomotionTechnique;
    private SwitchShader _switchShader;
    private WriteToFile _writeToFile;

    private const float FadeTime = 0.5f;

    private readonly Vector3 _playerResetRoom1 = new Vector3(0, 0, 0);
    private readonly Vector3 _playerResetRoom2 = new Vector3(20.8f, 0, 31.2f);
    private readonly Vector3 _playerResetRoom3 = new Vector3(41, 0, 60.4f);

    private bool _gameStart;
    private bool _first = true;

    private void Awake()
    {
        if (_gameStart) return;
        ExperimentManagerVariable = this;

        _locomotionManager = FindObjectOfType<LocomotionManager>();
        _switchShader = FindObjectOfType<SwitchShader>();
        _writeToFile = gameObject.AddComponent<WriteToFile>();  

        ActionSet.Activate(SteamVR_Input_Sources.Any, 0, true);       

        _gameStart = true;
    }

    private void Start()
    {
        GotoDefaultRoom();
    }

    private void Update()
    {
        CheckForResetPlayer();
    }

    private void CheckForResetPlayer()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            switch (DistanceManager.DistanceManagerVariable.GetActiveRoom())
            {
                case 3:
                    StartCoroutine(ResetPlayer(_playerResetRoom3));
                    break;
                case 2:
                    StartCoroutine(ResetPlayer(_playerResetRoom2));
                    break;
                case 1:
                    StartCoroutine(ResetPlayer(_playerResetRoom1));
                    break;
                case 0:
                    StartCoroutine(ResetPlayer(PlayerStartDefaultPosition));
                    break;
            }
        }
    }

    private IEnumerator ResetPlayer(Vector3 position)
    {
        SteamVR_Fade.Start(Color.black, FadeTime, true);

        // Apply translation 
        yield return new WaitForSeconds(FadeTime);
        Player.transform.position = position;

        // Fade to clear
        SteamVR_Fade.Start(Color.clear, FadeTime, true);
    }

    #region Highhight
    public void HighlightCube(GameObject cube)
    {
        var cubeRenderer = cube.GetComponent<Renderer>();
        _switchShader.SwitchToHighlightCube(cubeRenderer);
    }
    public void HighlightButton(GameObject buttonTop, GameObject buttonStand)
    {
        var rendererTop = buttonTop.GetComponent<Renderer>();
        var rendererStand = buttonStand.GetComponent<Renderer>();
        _switchShader.SwitchToHighlightButton(rendererTop, rendererStand);
    }
    public void HighlightHandle(GameObject handle)
    {
        var handleRenderer = handle.GetComponent<Renderer>();
        _switchShader.SwitchToHighlightHandle(handleRenderer);
    }

    public void HighlightTable(GameObject table)
    {
        var tableRenderer = table.GetComponent<Renderer>();
        _switchShader.SwitchToHighlightTable(tableRenderer);
    }
    #endregion

    #region Remove Highlight

    public void RemoveHighlightFromCube(GameObject cube)
    {
        var cubeRenderer = cube.GetComponent<Renderer>();
        _switchShader.SwitchToStandardCube(cubeRenderer);
    }

    public void RemoveHighlightFromButton(GameObject buttonTop, GameObject buttonStand)
    {
        var rendererTop = buttonTop.GetComponent<Renderer>();
        var rendererStand = buttonStand.GetComponent<Renderer>();
        _switchShader.SwitchToStandardButton(rendererTop, rendererStand);
    }

    public void RemoveHighlightFromHandle(GameObject handle)
    {
        var handleRenderer = handle.GetComponent<Renderer>();
        _switchShader.SwitchToStandardHandle(handleRenderer);
    }

    public void RemoveHighlightFromTable(GameObject table)
    {
        var tableRenderer = table.GetComponent<Renderer>();
        _switchShader.SwitchToStandardTable(tableRenderer);
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

    public void GotoDefaultRoom()
    {
        DistanceManager.DistanceManagerVariable.SetActiveRoomForDistance(0);
        SceneManager.LoadSceneAsync(4, LoadSceneMode.Additive);
        _locomotionManager.TurnOffAllTechniques();
        if (!_first)
        {
            _writeToFile.WriteAllDataToFile(CurrentLocomotionTechnique);
            UnloadScene(3);
        }
        SetPlayerToDefaultStartPosition();
    }

    public void StartTestWithNewTechnique()
    {
        DistanceManager.DistanceManagerVariable.ResetDistanceData();
        if (_first)
        {
            CurrentLocomotionTechnique = _locomotionManager.GetDummyLocomotionTechnique();
            _first = false;
        }
        else
            CurrentLocomotionTechnique = _locomotionManager.GetRandomLocomotionTechnique();

        if (CurrentLocomotionTechnique == LocomotionManager.LocomotionTechnique.None)
            QuitGame();
        StartCoroutine(StartTest());
    }

    private IEnumerator StartTest()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        yield return new WaitForSeconds(0.5f);
        SetPlayerToTestStartPosition();
        DistanceManager.DistanceManagerVariable.SetActiveRoomForDistance(1);
        UnloadScene(4);
    }

    private void SetPlayerToTestStartPosition()
    {
        Player.transform.position = PlayerStartTestPosition;
        Player.transform.rotation = PlayerStartTestRotation;
    }

    private void SetPlayerToDefaultStartPosition()
    {
        Player.transform.position = PlayerStartDefaultPosition;
        Player.transform.rotation = PlayerStartDefaultRotation;
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
