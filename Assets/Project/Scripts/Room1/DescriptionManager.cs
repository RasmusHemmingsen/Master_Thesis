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
        Text.text = "Teleport \n" +
            "To use this technique point the right controller against the ground \n" +
            "then a purple object appears and if you press on the touchpad \n" +
            "on the right controller and you will teleport to the object";
    }

    private void SetToDashStepDescription()
    {
        Text.text = "Dash Step \n" + 
            "To use this technique press the left touchpad \n" + 
            "and you will dash a step in the direction you are looking";
    }

    private void SetToBlinkStepDescription()
    {
        Text.text = "Blink Step \n" +
            "To use this technique press the left touchpad \n" +
            "and you will blink a step in the direction you are looking";
    }

    private void SetToArmswingDescription()
    {
        Text.text = "Armswing \n" +
            "To use this technique press the button on the side of the controller \n" +
            "and swing controller forwards and backwards to move in the direction \n" +
            "you are looking. This works independently for each controller";
    }

    private void SetToCybershoesDescription()
    {
        Text.text = "Cybershoes \n" +
            "To use this technique move your feet forward and backward like normal walking";

    }

    private void SetToSmoothLocomotionDescription()
    {
        Text.text = "Smooth locomotion \n" +
            "To use this technique hold the left touchpad \n" +
            "and you will walk in the direction you are looking, \n" + 
            "this can be used to go backwards too if the touchpad is pressed un the lower half";
    }

}
