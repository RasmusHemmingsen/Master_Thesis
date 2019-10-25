using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DashStep : MonoBehaviour
{
    public SteamVR_Action_Boolean m_DashAction = null;
    public GameObject m_Player;
    public Transform m_Camera;

    private SteamVR_Behaviour_Pose m_Pose = null;

    public float m_DashRange = 1f;
    public float m_DashTime = 0.1f;
    public bool m_IsEnabled = false;
    private bool m_IsDashing = false;

    [SerializeField]
    private Animator maskAnimator;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();

        m_DashAction[SteamVR_Input_Sources.Any].onStateUp += TryDash;
    }

    private void TryDash(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        if (!m_IsEnabled || m_IsDashing)
            return;

        Vector3 direction = m_Camera.forward;
        direction.y = 1;

        Ray ray = new Ray(m_Player.transform.position, direction);
        if (!Physics.Raycast(ray, out _, m_DashRange))
        {
                StartCoroutine(DoDash(direction));
        }
    }

    private IEnumerator DoDash(Vector3 direction)
    {
        m_IsDashing = true;

        direction.y = 0;
        if (maskAnimator != null)
            maskAnimator.SetBool("Mask", true);

        yield return new WaitForSeconds(0.1f);

        float elapsed = 0f;

        Vector3 startPoint = m_Player.transform.position;

        while (elapsed < m_DashTime)
        {
            elapsed += Time.deltaTime;
            float elapsedPct = elapsed / m_DashTime;

            m_Player.transform.position = Vector3.Lerp(startPoint, startPoint + (m_DashRange * direction), elapsedPct);
            yield return null;
        }

        if (maskAnimator != null)
            maskAnimator.SetBool("Mask", false);

        m_IsDashing = false;
    }

}
