using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager m_TimeManager;

    private float m_LastCheckpointTotal = 0f;

    private float m_Room1StartTime = 0f;
    private float m_Room1Time;

    private float m_Room2StartTime = 0f;
    private float m_Room2FirstButton;
    private float m_Room2SecondButton;
    private float m_Room2LastSection;
    private float m_Room2Time;

    private float m_Room3StartTime = 0f;
    private float m_Room3GrapCube1;
    private float m_Room3DeliverCube1;
    private float m_Room3GrapCube2;
    private float m_Room3DeliverCube2;
    private float m_Room3GrapCube3;
    private float m_Room3DeliverCube3;
    private float m_Room3GrapCube4;
    private float m_Room3DeliverCube4;
    private float m_Room3GrapCube5;
    private float m_Room3DeliverCube5;
    private float m_Room3GrapCube6;
    private float m_Room3DeliverCube6;
    private float m_Room3GrapCube7;
    private float m_Room3DeliverCube7;
    private float m_Room3GrapCube8;
    private float m_Room3DeliverCube8;
    private float m_Room3Time;

    private void Awake()
    {
        m_TimeManager = this;
    }

    public void StartTimerRoom1()
    {
        if (m_Room1StartTime == 0f)
            m_Room1StartTime = Time.time;
    }

    public void StopTimerRoom1()
    {
        m_Room1Time = Time.time - m_Room1StartTime;
    }

    public void StartTimerRoom2()
    {
        if (m_Room2StartTime == 0f)
        {
            m_Room2StartTime = Time.time;
            m_LastCheckpointTotal = m_Room2StartTime;
        }
    }

    public void StopTimerRoom2Button(int button)
    {
        float time = Time.time;

        switch(button)
        {
            case 1:
                m_Room2FirstButton = time - m_LastCheckpointTotal;
                break;
            case 2:
                m_Room2SecondButton = time - m_LastCheckpointTotal;
                break;
        }

        m_LastCheckpointTotal = time;
    }

    public void StopTimerRoom2()
    {
        m_Room2Time = Time.time - m_Room2StartTime;
        m_Room2LastSection = m_Room2Time - m_LastCheckpointTotal;
    }

    public void StartTimerRoom3()
    {
        if (m_Room3StartTime == 0f)
        {
            m_Room3StartTime = Time.time;
            m_LastCheckpointTotal = m_Room3StartTime;
        }
    }

    public void StopTimerRoom3Cube(int cube, bool grap)
    {
        float time = Time.time;

        switch (cube)
        {
            case 1:
                if(grap)
                    m_Room3GrapCube1 = time - m_LastCheckpointTotal;
                else
                    m_Room3DeliverCube1 = time - m_LastCheckpointTotal;
                break;
            case 2:
                if (grap)
                    m_Room3GrapCube2 = time - m_LastCheckpointTotal;
                else
                    m_Room3DeliverCube2 = time - m_LastCheckpointTotal;
                break;
            case 3:
                if (grap)
                    m_Room3GrapCube3 = time - m_LastCheckpointTotal;
                else
                    m_Room3DeliverCube3 = time - m_LastCheckpointTotal;
                break;
            case 4:
                if (grap)
                    m_Room3GrapCube4 = time - m_LastCheckpointTotal;
                else
                    m_Room3DeliverCube4 = time - m_LastCheckpointTotal;
                break;
            case 5:
                if (grap)
                    m_Room3GrapCube5 = time - m_LastCheckpointTotal;
                else
                    m_Room3DeliverCube5 = time - m_LastCheckpointTotal;
                break;
            case 6:
                if (grap)
                    m_Room3GrapCube6 = time - m_LastCheckpointTotal;
                else
                    m_Room3DeliverCube6 = time - m_LastCheckpointTotal;
                break;
            case 7:
                if (grap)
                    m_Room3GrapCube7 = time - m_LastCheckpointTotal;
                else
                    m_Room3DeliverCube7 = time - m_LastCheckpointTotal;
                break;
            case 8:
                if (grap)
                    m_Room3GrapCube8 = time - m_LastCheckpointTotal;
                else
                {
                    m_Room3DeliverCube8 = time - m_LastCheckpointTotal;
                    m_Room3Time = m_LastCheckpointTotal + m_Room3DeliverCube8;
                }
                    
                break;
        }

        m_LastCheckpointTotal = time;
    }

    public List<float> GetRoomTimer1()
    {
        List<float> list = new List<float>
        {
            m_Room1Time 
        };

        return list;
    }

    public List<float> GetRoomTimer2()
    {
        List<float> list = new List<float>
        {
            m_Room2Time,
            m_Room2FirstButton,
            m_Room2SecondButton,
            m_Room2LastSection
        };

        return list;
    }

    public List<float> GetRoomTimer3()
    {
        List<float> list = new List<float>
        {
            m_Room3Time,
            m_Room3GrapCube1,
            m_Room3DeliverCube1,
            m_Room3GrapCube2,
            m_Room3DeliverCube2,
            m_Room3GrapCube3,
            m_Room3DeliverCube3,
            m_Room3GrapCube4,
            m_Room3DeliverCube4,
            m_Room3GrapCube5,
            m_Room3DeliverCube5,
            m_Room3GrapCube6,
            m_Room3DeliverCube6,
            m_Room3GrapCube7,
            m_Room3DeliverCube7,
            m_Room3GrapCube8,
            m_Room3DeliverCube8
        };

        return list;
    }
}
