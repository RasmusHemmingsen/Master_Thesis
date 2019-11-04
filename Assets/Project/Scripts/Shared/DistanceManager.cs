using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceManager : MonoBehaviour
{
    public static DistanceManager DistanceManagerVariable;

    public Transform Player;

    public float XoffsetRoom1 = 3.5f;
    public float ZoffsetRoom1 = 3f;
    public float XoffsetRoom2 = 3.5f;
    public float ZoffsetRoom2 = -3.5f;
    public float XoffsetRoom3 = -25f;
    public float ZoffsetRoom3 = -53f;

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
        var xData = data.x + XoffsetRoom1;
        var zData = data.y + ZoffsetRoom1;

        _room1DistanceData += xData + ", " + zData + "\n";
    }

    public void WriteDistanceDataToRoom2(Vector2 data)
    {
        var xData = data.x + XoffsetRoom2;
        var zData = data.y + ZoffsetRoom2;

        _room2DistanceData += xData + ", " + zData + "\n";
    }

    public void WriteDistanceDataToRoom3(Vector2 data)
    {
        var xData = data.x + XoffsetRoom3;
        var zData = data.y + ZoffsetRoom3;

        _room3DistanceData += xData + ", " + zData + "\n";
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
