using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BlinkStep : MonoBehaviour
{
    public SteamVR_Action_Boolean m_BlinkAction = null;
    public GameObject m_Player;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private bool m_CanBlink = false;
    private bool m_IsBlinking = false;
    private float m_FadeTime = 0.2f;
    private float m_BlinkRange = 0.5f;
    public bool m_IsEnabled = false;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();

        m_BlinkAction[SteamVR_Input_Sources.Any].onStateUp += TryBlink;
    }

    private void TryBlink(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        if (!m_IsEnabled)
            return;

        Vector3 orientationEuler = new Vector3(0, m_Player.transform.eulerAngles.y, 0);
        Quaternion orientation = Quaternion.Euler(orientationEuler);

        RaycastHit hit;
        Ray ray = new Ray(m_Player.transform.position, orientation * m_Player.transform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.distance > m_BlinkRange)
            {
                StartCoroutine(DoDash(orientation));
            }
        }
    }

    private IEnumerator DoDash(Quaternion orientation)
    {
        // Flag 
        m_IsBlinking = true;

        // Fade to black
        SteamVR_Fade.Start(Color.black, m_FadeTime, true);

        // Apply translation 
        yield return new WaitForSeconds(m_FadeTime);
        m_Player.transform.position += (orientation * (m_BlinkRange * Vector3.forward));

        // Fade to clear
        SteamVR_Fade.Start(Color.clear, m_FadeTime, true);

        // De-flag
        m_IsBlinking = false;
    }
}
