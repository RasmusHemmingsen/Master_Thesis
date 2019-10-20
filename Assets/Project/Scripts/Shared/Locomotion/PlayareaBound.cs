using System;
using UnityEngine;
using Valve.VR;

public class PlayareaBound : MonoBehaviour
{
    public GameObject m_Camera;
    public Transform m_Player;

    public float m_PlayArea = 1f;

    private bool m_Isblack = false;

    private float m_FadeTime = 0.5f;

    private float m_OffsetX = 3.5f;
    private float m_OffsetZ = 3.5f;


    void Start()
    {
        m_PlayArea *= m_PlayArea;
        EnableNormalScreen();
    }

    void Update()
    {
        UpdateOffset();
        float distance = Mathf.Pow(GetXValue(), 2f) + Mathf.Pow(GetZValue(), 2f);
        if(distance > m_PlayArea)
        {
            if(!m_Isblack)
            {
                EnableBlackScreen();
            }
        }
        else if(m_Isblack)
        {
            EnableNormalScreen();
        }
    }

    private void UpdateOffset()
    {
        m_OffsetX = m_Player.position.x;
        m_OffsetZ = m_Player.position.z;
    }

    private float GetXValue()
    {
        float xPosition = m_Camera.transform.position.x + m_OffsetX;
        return xPosition;
    }

    private float GetZValue()
    {
        float zPosition = m_Camera.transform.position.z + m_OffsetZ;
        return zPosition;
    }

    private void EnableBlackScreen()
    {
        SteamVR_Fade.Start(Color.black, m_FadeTime, true); 
        m_Isblack = true;
    }

    private void EnableNormalScreen()
    {
        SteamVR_Fade.Start(Color.clear, m_FadeTime, true);
        m_Isblack = false;
    }
}
