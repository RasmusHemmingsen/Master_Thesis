using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour
{
    public enum LocomotionTechnique
    {
        BlinkStep,
        Teleport,
        DashStep,
        Armswing,
        SmoothLocomotion
    }

    public LocomotionTechnique dropDownValue = LocomotionTechnique.Teleport;  // this public var should appear as a drop down

    public void Pickup()
    {
        if (!ExperimentManager.ExperimentManagerVariable.isDemo)
        {
            ExperimentManager.ExperimentManagerVariable.StartTestWithNewTechnique();
        }
        else
        {
            int value;
            switch (dropDownValue)
            {
                case LocomotionTechnique.Armswing:
                    value = 1;
                    break;
                case LocomotionTechnique.DashStep:
                    value = 2;
                    break;
                case LocomotionTechnique.SmoothLocomotion:
                    value = 3;
                    break;
                case LocomotionTechnique.Teleport:
                    value = 4;
                    break;
                default: // defaults to blinkstep
                    value = 0;
                    break;
            }
            ExperimentManager.ExperimentManagerVariable.StartDemoWithTechnique(value);
        }
        
    }

    public void Drop()
    {

    }

}
