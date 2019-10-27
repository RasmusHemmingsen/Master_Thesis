using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BlinkStep : MonoBehaviour
{
    public SteamVR_Action_Boolean BlinkAction = null;
    public GameObject Player;
    public Transform Camera;

    private SteamVR_Behaviour_Pose _pose = null;
    private const float FadeTime = 0.2f;
    public float BlinkRange = 1.5f;
    public bool IsEnabled = false;
    private bool _isBlinking = false;

    private void Awake()
    {
        _pose = GetComponent<SteamVR_Behaviour_Pose>();

        BlinkAction[SteamVR_Input_Sources.Any].onStateUp += TryBlink;
    }

    private void TryBlink(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        if (!IsEnabled || _isBlinking)
            return;

        var direction = Camera.forward;
        direction.y = 0;
        var rayPosition = Player.transform.position;
        rayPosition.y = 1;

        var ray = new Ray(rayPosition, direction);
        if (!Physics.Raycast(ray, out _, BlinkRange))
        {
                StartCoroutine(DoBlink(direction));
        }
    }

    private IEnumerator DoBlink(Vector3 direction)
    {
        // Flag 
        _isBlinking = true;

        // Fade to black
        SteamVR_Fade.Start(Color.black, FadeTime, true);

        // Apply translation 
        yield return new WaitForSeconds(FadeTime);
        Player.transform.position += (BlinkRange * direction);

        // Fade to clear
        SteamVR_Fade.Start(Color.clear, FadeTime, true);

        // De-flag
        _isBlinking = false;
    }
}
