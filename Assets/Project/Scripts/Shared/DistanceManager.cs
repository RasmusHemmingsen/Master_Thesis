using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceManager : MonoBehaviour
{
    public static DistanceManager m_DistanceManager;

    public Transform m_Player;

    private int m_RoomActiveForDistance = 0;

    private string m_Room1DistanceData = "";
    private string m_Room2DistanceData = "";
    private string m_Room3DistanceData = "";


    private void Awake()
    {
        m_DistanceManager = this;
    }

    private void Start()
    {
        InvokeRepeating("SendDistanceData", 0.5f, 0.5f);
    }

    public void SetActiveRoomForDistance(int value)
    {
        m_RoomActiveForDistance = value;
    }

    public void ResetDistanceData()
    {
        m_Room1DistanceData = "";
        m_Room2DistanceData = "";
        m_Room3DistanceData = "";
        m_RoomActiveForDistance = 0;
    }

    private void SendDistanceData()
    {
        Vector2 position = new Vector2(m_Player.position.x, m_Player.position.z);
        switch (m_RoomActiveForDistance)
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
        m_Room1DistanceData += data.x.ToString() + ", " + data.y.ToString() + "\n";
    }

    public void WriteDistanceDataToRoom2(Vector2 data)
    {
        m_Room2DistanceData += data.x.ToString() + ", " + data.y.ToString() + "\n";
    }

    public void WriteDistanceDataToRoom3(Vector2 data)
    {
        m_Room3DistanceData += data.x.ToString() + ", " + data.y.ToString() + "\n";
    }

    public int GetActiveRoom()
    {
        return m_RoomActiveForDistance;
    }

    public string GetDistanceDataRoom1()
    {
        return m_Room1DistanceData;
    }

    public string GetDistanceDataRoom2()
    {
        return m_Room2DistanceData;
    }

    public string GetDistanceDataRoom3()
    {
        return m_Room3DistanceData;
    }
}
