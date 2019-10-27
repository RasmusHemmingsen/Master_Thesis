using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager TimeManagerVariable;

    private float _lastCheckpointTotal = 0f;

    private float _room1StartTime = 0f;
    private float _room1Time;

    private float _room2StartTime = 0f;
    private float _room2FirstButton;
    private float _room2SecondButton;
    private float _room2LastSection;
    private float _room2Time;

    private float _room3StartTime = 0f;
    private float _room3GrapCube1;
    private float _room3DeliverCube1;
    private float _room3GrapCube2;
    private float _room3DeliverCube2;
    private float _room3GrapCube3;
    private float _room3DeliverCube3;
    private float _room3GrapCube4;
    private float _room3DeliverCube4;
    private float _room3GrapCube5;
    private float _room3DeliverCube5;
    private float _room3GrapCube6;
    private float _room3DeliverCube6;
    private float _room3GrapCube7;
    private float _room3DeliverCube7;
    private float _room3GrapCube8;
    private float _room3DeliverCube8;
    private float _room3Time;

    private void Awake()
    {
        TimeManagerVariable = this;
    }

    public void StartTimerRoom1()
    {
            _room1StartTime = Time.time;
    }

    public void StopTimerRoom1()
    {
        _room1Time = Time.time - _room1StartTime;
    }

    public void StartTimerRoom2()
    {
            _room2StartTime = Time.time;
            _lastCheckpointTotal = _room2StartTime;
    }

    public void StopTimerRoom2Button(int button)
    {
        float time = Time.time;

        switch(button)
        {
            case 1:
                _room2FirstButton = time - _lastCheckpointTotal;
                break;
            case 2:
                _room2SecondButton = time - _lastCheckpointTotal;
                break;
        }

        _lastCheckpointTotal = time;
    }

    public void StopTimerRoom2()
    {
        var time = Time.time;

        _room2Time = time - _room2StartTime;
        _room2LastSection = time - _lastCheckpointTotal;
    }

    public void StartTimerRoom3()
    {
            _room3StartTime = Time.time;
            _lastCheckpointTotal = _room3StartTime;
    }

    public void StopTimerRoom3Cube(int cube, bool grap)
    {
        var time = Time.time;

        switch (cube)
        {
            case 1:
                if(grap)
                    _room3GrapCube1 = time - _lastCheckpointTotal;
                else
                    _room3DeliverCube1 = time - _lastCheckpointTotal;
                break;
            case 2:
                if (grap)
                    _room3GrapCube2 = time - _lastCheckpointTotal;
                else
                    _room3DeliverCube2 = time - _lastCheckpointTotal;
                break;
            case 3:
                if (grap)
                    _room3GrapCube3 = time - _lastCheckpointTotal;
                else
                    _room3DeliverCube3 = time - _lastCheckpointTotal;
                break;
            case 4:
                if (grap)
                    _room3GrapCube4 = time - _lastCheckpointTotal;
                else
                    _room3DeliverCube4 = time - _lastCheckpointTotal;
                break;
            case 5:
                if (grap)
                    _room3GrapCube5 = time - _lastCheckpointTotal;
                else
                    _room3DeliverCube5 = time - _lastCheckpointTotal;
                break;
            case 6:
                if (grap)
                    _room3GrapCube6 = time - _lastCheckpointTotal;
                else
                    _room3DeliverCube6 = time - _lastCheckpointTotal;
                break;
            case 7:
                if (grap)
                    _room3GrapCube7 = time - _lastCheckpointTotal;
                else
                    _room3DeliverCube7 = time - _lastCheckpointTotal;
                break;
            case 8:
                if (grap)
                    _room3GrapCube8 = time - _lastCheckpointTotal;
                else
                {
                    _room3DeliverCube8 = time - _lastCheckpointTotal;
                    _room3Time = time - _room3StartTime;
                }
                    
                break;
        }

        _lastCheckpointTotal = time;
    }

    public List<float> GetRoomTimer1()
    {
        var list = new List<float>
        {
            _room1Time 
        };

        return list;
    }

    public List<float> GetRoomTimer2()
    {
        var list = new List<float>
        {
            _room2Time,
            _room2FirstButton,
            _room2SecondButton,
            _room2LastSection
        };

        return list;
    }

    public List<float> GetRoomTimer3()
    {
        var list = new List<float>
        {
            _room3Time,
            _room3GrapCube1,
            _room3DeliverCube1,
            _room3GrapCube2,
            _room3DeliverCube2,
            _room3GrapCube3,
            _room3DeliverCube3,
            _room3GrapCube4,
            _room3DeliverCube4,
            _room3GrapCube5,
            _room3DeliverCube5,
            _room3GrapCube6,
            _room3DeliverCube6,
            _room3GrapCube7,
            _room3DeliverCube7,
            _room3GrapCube8,
            _room3DeliverCube8
        };

        return list;
    }
}
