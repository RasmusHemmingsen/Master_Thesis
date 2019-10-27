using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceManager : MonoBehaviour
{
    public static DistanceManager DistanceManagerVariable;

    public Transform Player;

    private int _roomActiveForDistance = 0;

    private string _room1DistanceData = "";
    private string _room2DistanceData = "";
    private string _room3DistanceData = "";


    private void Awake()
    {
        DistanceManagerVariable = this;
    }

    private void Start()
    {
        InvokeRepeating("SendDistanceData", 0.5f, 0.5f);
    }

    public void SetActiveRoomForDistance(int value)
    {
        _roomActiveForDistance = value;
    }

    public void ResetDistanceData()
    {
        _room1DistanceData = "";
        _room2DistanceData = "";
        _room3DistanceData = "";
        _roomActiveForDistance = 0;
    }

    private void SendDistanceData()
    {
        var position = new Vector2(Player.position.x, Player.position.z);
        switch (_roomActiveForDistance)
        {
            case 3:
                WriteDistanceDataToRoom3(position);
                break;
            case 2:
                WriteDistanceDataToRoom2(position);
                break;
            case 1:
                WriteDistanceDataToRoom1(position);
                break;
        }
    }

    public void WriteDistanceDataToRoom1(Vector2 data)
    {
        _room1DistanceData += data.x + ", " + data.y + "\n";
    }

    public void WriteDistanceDataToRoom2(Vector2 data)
    {
        _room2DistanceData += data.x + ", " + data.y + "\n";
    }

    public void WriteDistanceDataToRoom3(Vector2 data)
    {
        _room3DistanceData += data.x + ", " + data.y + "\n";
    }

    public int GetActiveRoom()
    {
        return _roomActiveForDistance;
    }

    public string GetDistanceDataRoom1()
    {
        return _room1DistanceData;
    }

    public string GetDistanceDataRoom2()
    {
        return _room2DistanceData;
    }

    public string GetDistanceDataRoom3()
    {
        return _room3DistanceData;
    }
}
