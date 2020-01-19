using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room3Manager : MonoBehaviour
{
    public List<GameObject> ListOfCubes;
    public GameObject Handle;
    public GameObject Table;

    private GameObject _currentCube;
    private GameObject _nextCube;

    private int _numberOfCubesCorrectPlaced = 0;
    private int _cubeListSize;

    private void Start()
    {
        _cubeListSize = ListOfCubes.Count;
        HighlightHandle();
        _currentCube = ListOfCubes[_numberOfCubesCorrectPlaced];
        _nextCube = GetNextCubeIfThereIsOne();   
    }

    public void StartTimer()
    {
        TimeManager.TimeManagerVariable.StartTimerRoom3();
    }

    private void HighlightHandle()
    {
        ExperimentManager.ExperimentManagerVariable.HighlightHandle(Handle);
    }

    public void HandlePressed()
    {
        ExperimentManager.ExperimentManagerVariable.RemoveHighlightFromHandle(Handle);
        TimeManager.TimeManagerVariable.StopTimerRoom2();
        DistanceManager.DistanceManagerVariable.SetActiveRoomForDistance(3);
    }

    public void HighlightCurrentCube()
    {
        ExperimentManager.ExperimentManagerVariable.HighlightCube(_currentCube);
    }

    private void RemoveHighlightFromCurrentCubeAndTable()
    {
        ExperimentManager.ExperimentManagerVariable.RemoveHighlightFromCube(_currentCube);
        ExperimentManager.ExperimentManagerVariable.RemoveHighlightFromTable(Table);
    }

    public void HighlightTable()
    {
        ExperimentManager.ExperimentManagerVariable.HighlightTable(Table);
    }

    public void CurrentCubePlacedCorrectly()
    {
        if(_numberOfCubesCorrectPlaced == 0)
        {
            Handle.GetComponent<Door>().StartCloseDoorAnimation();
            ExperimentManager.ExperimentManagerVariable.UnloadScene(2);
        }

        RemoveHighlightFromCurrentCubeAndTable();
        _numberOfCubesCorrectPlaced += 1;      

        TimeManager.TimeManagerVariable.StopTimerRoom3Cube(_numberOfCubesCorrectPlaced, false);

        if (_numberOfCubesCorrectPlaced >= _cubeListSize)
        {
            ExperimentManager.ExperimentManagerVariable.GotoDefaultRoom(); 
        }

       

        if(_nextCube != null)
        {
            StartCoroutine(DestroyGameObject(_currentCube));
            _currentCube = _nextCube;
            _nextCube = GetNextCubeIfThereIsOne();
        }

        HighlightCurrentCube();
        StartCoroutine(AddInteracableTagToCube(_currentCube));
    }

    private IEnumerator DestroyGameObject(GameObject gameObject)
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    private IEnumerator AddInteracableTagToCube(GameObject cube)
    {
        yield return null;
        cube.tag = "Interactable";
    }

    private GameObject GetNextCubeIfThereIsOne()
    {
        return _numberOfCubesCorrectPlaced + 1 >= _cubeListSize ? null : ListOfCubes[_numberOfCubesCorrectPlaced + 1];
    }

    public bool IsCurrentCube(GameObject cube)
    {
        return GameObject.ReferenceEquals(cube, _currentCube);
    }
}
