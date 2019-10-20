using UnityEngine;

public class PlayareaBound : MonoBehaviour
{
    public GameObject m_Camera;
    public GameObject m_CameraBlack;

    public float m_PlayArea = 1f;

    private bool m_Isblack = false;

    private float m_FadeTime = 0.5f;

    private float m_OffsetX = 3.5f;
    private float m_OffsetZ = 3.5f;

    private GameObject m_ActiveCamera;

    void Start()
    {
        m_PlayArea *= m_PlayArea;
        EnableNormalScreen();
    }

    void Update()
    {
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

    private float GetXValue()
    {
        float xPosition = m_ActiveCamera.transform.position.x + m_OffsetX;
        return xPosition;
    }

    private float GetZValue()
    {
        float zPosition = m_ActiveCamera.transform.position.z + m_OffsetZ;
        return zPosition;
    }

    private void EnableBlackScreen()
    {
        // SteamVR_Fade.Start(Color.black, m_FadeTime, true); 

        m_Camera.SetActive(false);
        m_CameraBlack.SetActive(true);
        m_Isblack = true;
        m_ActiveCamera = m_CameraBlack;
    }

    private void EnableNormalScreen()
    {
        // SteamVR_Fade.Start(Color.clear, m_FadeTime, true);

        m_Camera.SetActive(true);
        m_CameraBlack.SetActive(false);
        m_Isblack = false;
        m_ActiveCamera = m_Camera;
    }
}
