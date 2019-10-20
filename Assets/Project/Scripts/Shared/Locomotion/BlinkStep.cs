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
    private float m_FadeTime = 0.5f;
    private float m_BlinkRange = 1.0f;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();

        m_BlinkAction[SteamVR_Input_Sources.Any].onStateUp += TryBlink;
    }

    private void Update()
    {
        m_CanBlink = CanBlink();
    }

    private void TryBlink(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        // Check for valid position, and if already blinking
        if (!m_CanBlink || m_IsBlinking)
            return;

        // Move 
        StartCoroutine(Move(m_Player.transform));

    }

    private IEnumerator Move(Transform player)
    {
        // Flag 
        m_IsBlinking = true;

        // Fade to black
        SteamVR_Fade.Start(Color.black, m_FadeTime, true);

        // Apply translation 
        yield return new WaitForSeconds(m_FadeTime);
        player.position += player.transform.forward * m_BlinkRange;

        // Fade to clear
        SteamVR_Fade.Start(Color.clear, m_FadeTime, true);

        // De-flag
        m_IsBlinking = false;
    }

    private bool CanBlink()
    {
        RaycastHit hit;

        // Cast Ray
        Ray ray = new Ray(m_Player.transform.position, m_Player.transform.forward);

        // If it is a hit
        if (Physics.Raycast(ray, out hit) && hit.distance > m_BlinkRange)
        {
            return true;
        }

        // If not a hit
        return false;
    }
}
