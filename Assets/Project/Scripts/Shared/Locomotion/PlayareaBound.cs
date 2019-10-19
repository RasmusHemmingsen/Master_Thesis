using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayareaBound : MonoBehaviour
{
    public GameObject m_camera;
    public float m_PlayArea = 1f;

    private float m_FadeTime = 0.5f;
    private bool m_Isblack = false;
    // Start is called before the first frame update
    void Start()
    {
        m_PlayArea *= m_PlayArea;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Mathf.Pow(m_camera.transform.position.x, 2f) * Mathf.Pow(m_camera.transform.position.z, 2f);
        if(distance > m_PlayArea)
        {
            if(!m_Isblack)
            {
                SteamVR_Fade.Start(Color.black, m_FadeTime, true);
                m_Isblack = true;
            }
        }
        else if(m_Isblack)
        {
            SteamVR_Fade.Start(Color.clear, m_FadeTime, true);
            m_Isblack = false;
        }
    }
}
