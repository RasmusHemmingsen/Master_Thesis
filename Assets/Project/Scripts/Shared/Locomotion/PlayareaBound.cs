using UnityEngine;

public class PlayareaBound : MonoBehaviour
{
    public GameObject m_Camera;
    public GameObject m_CameraBlack;

    public float m_PlayArea = 0.1f;

    private bool m_Isblack = false;

    private float m_OffsetX = 3.5f;
    private float m_OffsetZ = 3.5f;

    private float m_XMax = 0.7f;
    private float m_ZMax = 0.7f;

    private GameObject m_ActiveCamera;

    void Start()
    {
        m_PlayArea *= m_PlayArea;
        EnableNormalScreen();
    }

    void Update()
    {
        float xValue = GetXValue();
        float zValue = GetZValue();

        float distance = Mathf.Pow(xValue, 2f) * Mathf.Pow(zValue, 2f);
        if(distance > m_PlayArea || xValue > m_XMax || zValue > m_ZMax || xValue < -m_XMax || zValue < m_ZMax)
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
        m_Camera.SetActive(false);
        m_CameraBlack.SetActive(true);
        m_Isblack = true;
        m_ActiveCamera = m_CameraBlack;
    }

    private void EnableNormalScreen()
    {
        m_Camera.SetActive(true);
        m_CameraBlack.SetActive(false);
        m_Isblack = false;
        m_ActiveCamera = m_Camera;
    }
}
