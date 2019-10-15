using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public Animator m_Animator;

    public void StartOpenDoorAnimation()
    {
        m_Animator.SetBool("OpenDoor", true);
    }
}
