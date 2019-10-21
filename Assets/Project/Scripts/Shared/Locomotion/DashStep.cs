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
    public bool m_IsEnabled = false;

    [SerializeField]
    private Animator maskAnimator;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();

        m_DashAction[SteamVR_Input_Sources.Any].onStateUp += TryDash;
    }

    private void TryDash(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        if (!m_IsEnabled)
            return;

        Vector3 orientationEuler = new Vector3(0, m_Player.transform.eulerAngles.y, 0);
        Quaternion orientation = Quaternion.Euler(orientationEuler);


        RaycastHit hit;
        Ray ray = new Ray(m_Player.transform.position, orientation * m_Player.transform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.distance > m_DashRange)
            {
                StartCoroutine(DoDash(orientation));
            }
        }
    }

    private IEnumerator DoDash(Quaternion orientation)
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

            m_Player.transform.position = Vector3.Lerp(startPoint, startPoint + (orientation * (m_DashRange * Vector3.forward)), elapsedPct);
            yield return null;
        }

        if (maskAnimator != null)
            maskAnimator.SetBool("Mask", false);
    }
}
