using System.Collections;
using UnityEngine;
using Valve.VR;

public class Teleporter : MonoBehaviour
{
    public SteamVR_Action_Boolean TeleportAction = null;
    public GameObject Player;
    public Transform Controller;

    [HideInInspector]
    public GameObject Pointer = null;

    private SteamVR_Behaviour_Pose _pose = null;
    private bool _hasPosition = false;
    private bool _isTeleporting = false;
    private const float FadeTime = 0.5f;
    public float TeleportMaxRange = 5.0f;

    private void Awake()
    {
        _pose = GetComponent<SteamVR_Behaviour_Pose>();

        TeleportAction[SteamVR_Input_Sources.Any].onStateUp += TryTeleport;
    }
 
    private void Update()
    {
        // pointer
        _hasPosition = UpdatePointer();
        Pointer.SetActive(_hasPosition);

    }

    private void TryTeleport(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        // Check for valid position, and if already teleporting
        if (!_hasPosition || _isTeleporting)
            return;

        // Get camera rig, and head position
        var cameraRig = SteamVR_Render.Top().origin;
        var headPosition = SteamVR_Render.Top().head.position;

        // Figure out translation
        var groundPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);
        var translateVector = Pointer.transform.position - groundPosition;

        // Move 
        StartCoroutine(Move(Player.transform, translateVector));

    }

    private IEnumerator Move(Transform player, Vector3 translation)
    {
        // Flag 
        _isTeleporting = true;

        // Fade to black
        SteamVR_Fade.Start(Color.black, FadeTime, true);

        // Apply translation 
        yield return new WaitForSeconds(FadeTime);
        player.position += translation;

        // Fade to clear
        SteamVR_Fade.Start(Color.clear, FadeTime, true);

        // De-flag
        _isTeleporting = false;
    }

    private bool UpdatePointer()
    {
        // Ray from the controller
        var ray = new Ray(Controller.position, Controller.forward);

        // If it is a hit
        if(Physics.Raycast(ray, out var hit) && hit.distance < TeleportMaxRange && hit.collider.CompareTag("CanTeleport"))
        {
            Pointer.transform.position = hit.point;
            return true;
        }

        // If not a hit
        return false;
    }
}
