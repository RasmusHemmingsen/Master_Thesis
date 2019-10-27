using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DashStep : MonoBehaviour
{
    public SteamVR_Action_Boolean DashAction = null;
    public GameObject Player;
    public Transform Camera;

    private SteamVR_Behaviour_Pose _pose = null;

    public float DashRange = 1f;
    public float DashTime = 0.1f;
    public bool IsEnabled = false;
    private bool _isDashing = false;

    [SerializeField]
    private Animator _maskAnimator;

    private void Awake()
    {
        _pose = GetComponent<SteamVR_Behaviour_Pose>();

        DashAction[SteamVR_Input_Sources.Any].onStateUp += TryDash;
    }

    private void TryDash(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        if (!IsEnabled || _isDashing)
            return;

        var direction = Camera.forward;
        direction.y = 0;
        var rayPosition = Player.transform.position;
        rayPosition.y = 1;

        var ray = new Ray(rayPosition, direction);
        if (!Physics.Raycast(ray, out _, DashRange))
        {
                StartCoroutine(DoDash(direction));
        }
    }

    private IEnumerator DoDash(Vector3 direction)
    {
        _isDashing = true;

        if (_maskAnimator != null)
            _maskAnimator.SetBool("Mask", true);

        yield return new WaitForSeconds(0.1f);

        var elapsed = 0f;

        var startPoint = Player.transform.position;

        while (elapsed < DashTime)
        {
            elapsed += Time.deltaTime;
            var elapsedPct = elapsed / DashTime;

            Player.transform.position = Vector3.Lerp(startPoint, startPoint + (DashRange * direction), elapsedPct);
            yield return null;
        }

        if (_maskAnimator != null)
            _maskAnimator.SetBool("Mask", false);

        _isDashing = false;
    }

}
