using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour
{
    public void Pickup()
    {
        ExpermentManager.m_ExpermentManager.StartTestWithNewTechnique();
    }

    public void Drop()
    {

    }
    
}
