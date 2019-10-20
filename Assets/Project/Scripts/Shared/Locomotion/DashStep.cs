using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DashStep : MonoBehaviour
{
    public SteamVR_Action_Boolean m_DashAction = null;
    public GameObject m_Player;

    private SteamVR_Behaviour_Pose m_Pose = null;

    public float m_DashRange = 0.5f;
    public float m_DashTime = 0.2f;

    [SerializeField]
    private Animator maskAnimator;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();

        m_DashAction[SteamVR_Input_Sources.Any].onStateUp += TryDash;
    }

    private void TryDash(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        RaycastHit hit;
        Ray ray = new Ray(m_Player.transform.position, m_Player.transform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.distance > m_DashRange)
            {
                StartCoroutine(DoDash());
            }
        }
    }

    private IEnumerator DoDash()
    {
        if (maskAnimator != null)
            maskAnimator.SetBool("Mask", true);

        yield return new WaitForSeconds(0.1f);

        float elapsed = 0f;

        Vector3 startPoint = m_Player.transform.position;

        while (elapsed < m_DashTime)
        {
            elapsed += Time.deltaTime;
            float elapsedPct = elapsed / m_DashTime;

            m_Player.transform.position = Vector3.Lerp(startPoint, m_Player.transform.position* m_DashRange, elapsedPct);
            yield return null;
        }

        if (maskAnimator != null)
            maskAnimator.SetBool("Mask", false);
    }
}
