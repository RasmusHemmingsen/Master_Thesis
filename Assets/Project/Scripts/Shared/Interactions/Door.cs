using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator Animator;

    public void StartOpenDoorAnimation()
    {
        Animator.SetBool("HandleTriggered", true);
    }

    public void StartCloseDoorAnimation()
    {
        Animator.SetBool("CloseDoor", true);
    }
}
