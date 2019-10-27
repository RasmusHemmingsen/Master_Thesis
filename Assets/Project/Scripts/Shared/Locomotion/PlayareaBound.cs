using System;
using UnityEngine;
using Valve.VR;

public class PlayareaBound : MonoBehaviour
{
    public GameObject Camera;
    public Transform Player;

    public float PlayArea = 1f;

    private bool _isblack = false;

    private const float FadeTime = 0.5f;

    private float _offsetX = 3.5f;
    private float _offsetZ = 3.5f;


    void Start()
    {
        PlayArea *= PlayArea;
        EnableNormalScreen();
    }

    void Update()
    {
        UpdateOffset();
        var distance = Mathf.Pow(GetXValue(), 2f) + Mathf.Pow(GetZValue(), 2f);
        if(distance > PlayArea)
        {
            if(!_isblack)
            {
                EnableBlackScreen();
            }
        }
        else if(_isblack)
        {
            EnableNormalScreen();
        }
    }

    private void UpdateOffset()
    {
        _offsetX = -Player.position.x;
        _offsetZ = -Player.position.z;
    }

    private float GetXValue()
    {
        var xPosition = Camera.transform.position.x + _offsetX;
        return xPosition;
    }

    private float GetZValue()
    {
        var zPosition = Camera.transform.position.z + _offsetZ;
        return zPosition;
    }

    private void EnableBlackScreen()
    {
        SteamVR_Fade.Start(Color.black, FadeTime, true); 
        _isblack = true;
    }

    private void EnableNormalScreen()
    {
        SteamVR_Fade.Start(Color.clear, FadeTime, true);
        _isblack = false;
    }
}
