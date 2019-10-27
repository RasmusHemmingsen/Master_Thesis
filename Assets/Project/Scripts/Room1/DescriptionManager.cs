using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionManager : MonoBehaviour
{

    public Text Text;

    public void SwitchDescription(LocomotionManager.LocomotionTechnique technique)
    {
        switch (technique)
        {
            case LocomotionManager.LocomotionTechnique.SmoothLocomotion:
                SetToSmoothLocomotionDescription();
                break;
            case LocomotionManager.LocomotionTechnique.Teleport:
                SetToTeleportDescription();
                break;
            case LocomotionManager.LocomotionTechnique.DashStep:
                SetToDashStepDescription();
                break;
            case LocomotionManager.LocomotionTechnique.Armswing:
                SetToArmswingDescription();
                break;
            case LocomotionManager.LocomotionTechnique.Cybershoes:
                SetToCybershoesDescription();
                break;
            case LocomotionManager.LocomotionTechnique.BlinkStep:
                SetToBlinkStepDescription();
                break;
            default:
                Text.text = "";
                break;
        }
    }

    private void SetToTeleportDescription()
    {
        Text.text = "Teleport \n";
    }

    private void SetToDashStepDescription()
    {
        Text.text = "Dash Step \n";
    }

    private void SetToBlinkStepDescription()
    {
        Text.text = "Blink Step \n";
    }

    private void SetToArmswingDescription()
    {
        Text.text = "Armswing \n";
    }

    private void SetToCybershoesDescription()
    {
        Text.text = "Cybershoes \n";

    }

    private void SetToSmoothLocomotionDescription()
    {
        Text.text = "Smooth locomotion \n";
    }

}
