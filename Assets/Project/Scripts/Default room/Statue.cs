using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour
{
    public void Pickup()
    {
        ExperimentManager.ExperimentManagerVariable.StartTestWithNewTechnique();
    }

    public void Drop()
    {

    }
    
}
