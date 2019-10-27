using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BlinkStep : MonoBehaviour
{
    public SteamVR_Action_Boolean m_BlinkAction = null;
    public GameObject m_Player;
    public Transform m_Camera;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private readonly float m_FadeTime = 0.2f;
    public float m_BlinkRange = 1.5f;
    public bool m_IsEnabled = false;
    private bool m_IsBlinking = false;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();

        m_BlinkAction[SteamVR_Input_Sources.Any].onStateUp += TryBlink;
    }

    private void TryBlink(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        if (!m_IsEnabled || m_IsBlinking)
            return;

        Vector3 direction = m_Camera.forward;
        direction.y = 0;
        Vector3 rayPosition = m_Player.transform.position;
        rayPosition.y = 1;

        Ray ray = new Ray(rayPosition, direction);
        if (!Physics.Raycast(ray, out _, m_BlinkRange))
        {
                StartCoroutine(DoBlink(direction));
        }
    }

    private IEnumerator DoBlink(Vector3 direction)
    {
        // Flag 
        m_IsBlinking = true;

        // Fade to black
        SteamVR_Fade.Start(Color.black, m_FadeTime, true);

        // Apply translation 
        yield return new WaitForSeconds(m_FadeTime);
        m_Player.transform.position += (m_BlinkRange * direction);

        // Fade to clear
        SteamVR_Fade.Start(Color.clear, m_FadeTime, true);

        // De-flag
        m_IsBlinking = false;
    }
}
