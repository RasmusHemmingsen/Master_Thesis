using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room2Manager : MonoBehaviour
{
    public GameObject WallToRoom1;
    public GameObject WallToRoom3;
    public GameObject Button1;
    public GameObject Button2;
    
    private bool _firstButtonPressed = false;
    private bool _secondButtonPressed = false;

    private void Start()
    {
        HighlightButton1();
        StartCoroutine(StartTimer());
    }
    
    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(1);
        TimeManager.TimeManagerVariable.StartTimerRoom2();
        DistanceManager.DistanceManagerVariable.SetActiveRoomForDistance(2);

    }

    private void StopTimerButton(int button)
    {
        TimeManager.TimeManagerVariable.StopTimerRoom2Button(button);
    }

    public void Button1Pressed()
    {
        if (_firstButtonPressed) return;
        RemoveHighlightFromButton1();
        CloseForRoom1();
        HighlightButton2();
        StopTimerButton(1);
        _firstButtonPressed = true;
    }

    public void Button2Pressed()
    {
        if (_secondButtonPressed || !_firstButtonPressed) return;
        OpenForRoom3();
        StopTimerButton(2);
        RemoveHighlightFromButton2();
        _secondButtonPressed = true;
    }

    public bool IsBothButtonsPressed()
    {
        return _firstButtonPressed && _secondButtonPressed;
    }

    public void HighlightButton1()
    {
        ExperimentManager.ExperimentManagerVariable.HighlightButton(Button1.transform.GetChild(0).gameObject, Button1.transform.GetChild(1).gameObject);
    }

    private void RemoveHighlightFromButton1()
    {
        ExperimentManager.ExperimentManagerVariable.RemoveHighlightFromButton(Button1.transform.GetChild(0).gameObject, Button1.transform.GetChild(1).gameObject);
    }

    private void HighlightButton2()
    {
        ExperimentManager.ExperimentManagerVariable.HighlightButton(Button2.transform.GetChild(0).gameObject, Button2.transform.GetChild(1).gameObject);
    }

    private void RemoveHighlightFromButton2()
    {
        ExperimentManager.ExperimentManagerVariable.RemoveHighlightFromButton(Button2.transform.GetChild(0).gameObject, Button2.transform.GetChild(1).gameObject);
    }

    private void OpenForRoom3()
    {
        if (!_firstButtonPressed) return;
        DisableWallToRoom3();
        LoadScene3();
    }

    private void DisableWallToRoom3()
    {
        WallToRoom3.GetComponent<MeshRenderer>().enabled = false;
        WallToRoom3.GetComponent<BoxCollider>().enabled = false;

    }

    private void LoadScene3()
    {
        SceneManager.LoadSceneAsync(3, LoadSceneMode.Additive);
    }

    private void CloseForRoom1()
    {
        EnableWallToRoom1();
        UnloadScene1();
    }

    private void EnableWallToRoom1()
    {
        WallToRoom1.GetComponent<MeshRenderer>().enabled = true;
        WallToRoom1.GetComponent<BoxCollider>().enabled = true;

    }

    private void UnloadScene1()
    {
        ExperimentManager.ExperimentManagerVariable.UnloadScene(1);
    }
}
