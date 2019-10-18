using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator m_Animator;

    public void StartOpenDoorAnimation()
    {
        m_Animator.SetBool("HandleTriggered", true);
    }

    public void StartCloseDoorAnimation()
    {
        m_Animator.SetBool("CloseDoor", true);
    }
}
