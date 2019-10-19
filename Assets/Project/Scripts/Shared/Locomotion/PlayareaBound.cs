using UnityEngine;

public class PlayareaBound : MonoBehaviour
{
    public GameObject m_camera;
    public GameObject m_cameraBlack;

    public float m_PlayArea = 0.1f;
    private bool m_Isblack = false;

    void Start()
    {
        m_PlayArea *= m_PlayArea;
    }

    void Update()
    {
        float distance = Mathf.Pow(m_camera.transform.position.x, 2f) * Mathf.Pow(m_camera.transform.position.z, 2f);
        if(distance > m_PlayArea)
        {
            if(!m_Isblack)
            {
                m_camera.SetActive(false);
                m_cameraBlack.SetActive(true);
                m_Isblack = true;
            }
        }
        else if(m_Isblack)
        {
            m_camera.SetActive(true);
            m_cameraBlack.SetActive(false);
            m_Isblack = false;
        }
    }
}
