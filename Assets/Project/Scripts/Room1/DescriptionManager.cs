using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionManager : MonoBehaviour
{
    public Text DescriptionText;

    public void SwitchDescription(LocomotionManager.LocomotionTechnique technique, bool defaultRoom)
    {
        switch (technique)
        {
            case LocomotionManager.LocomotionTechnique.SmoothLocomotion:
                SetToSmoothLocomotionDescription(defaultRoom);
                break;
            case LocomotionManager.LocomotionTechnique.Teleport:
                SetToTeleportDescription(defaultRoom);
                break;
            case LocomotionManager.LocomotionTechnique.DashStep:
                SetToDashStepDescription(defaultRoom);
                break;
            case LocomotionManager.LocomotionTechnique.Armswing:
                SetToArmswingDescription(defaultRoom);
                break;
            case LocomotionManager.LocomotionTechnique.Cybershoes:
                SetToCybershoesDescription(defaultRoom);
                break;
            case LocomotionManager.LocomotionTechnique.BlinkStep:
                SetToBlinkStepDescription(defaultRoom);
                break;
            default:
                DescriptionText.text = "";
                break;
        }
    }

    private void SetToTeleportDescription(bool defaultRoom)
    {
        string text = "Teleport \n" +
            "To use this technique, point the right controller against the ground \n" +
            "then a purple object appears and if you press on the touchpad \n" +
            "on the right controller and you will teleport to the object.";

        if(!defaultRoom)
        {
            text += "\n " + GetDescriptionOfRoom1();
        }
        DescriptionText.text = text;
    }

    private void SetToDashStepDescription(bool defaultRoom)
    {
        string text = "Dash Step \n" +
        "To use this technique, press the left touchpad \n" +
        "and you will dash a step in the direction you are looking.";

        if (!defaultRoom)
        {
            text += "\n " + GetDescriptionOfRoom1();
        }
        DescriptionText.text = text;
    }

    private void SetToBlinkStepDescription(bool defaultRoom)
    {
        string text = "Blink Step \n" +
        "To use this technique, press the left touchpad \n" +
        "and you will blink a step in the direction you are looking.";

        if (!defaultRoom)
        {
            text += "\n " + GetDescriptionOfRoom1();
        }
        DescriptionText.text = text;
    }

    private void SetToArmswingDescription(bool defaultRoom) 
    { 
        string text = "Armswing \n" +
        "To use this technique, hold the button on the side of the controller \n" +
        "and swing controller forwards and backwards to move in the direction \n" +
        "you are looking.";

        if (!defaultRoom)
        {
            text += "\n " + GetDescriptionOfRoom1();
        }
        DescriptionText.text = text;
    }

    private void SetToCybershoesDescription(bool defaultRoom)
    {
        string text = "Cybershoes \n" +
        "To use this technique, move your feet forward and backward like normal walking.";

        if (!defaultRoom)
        {
            text += "\n " + GetDescriptionOfRoom1();
        }
        DescriptionText.text = text;
    }

    private void SetToSmoothLocomotionDescription(bool defaultRoom)
    {
        string text = "Smooth locomotion \n" +
        "To use this technique, hold the left touchpad \n" +
        "and you will walk in the direction you are looking, \n" +
        "this can be used to go backwards if you press under the lower half of the touchpad. \n" +
        "If the button on the side of the controller is pressed, you rotate 90 degrees.";

        if (!defaultRoom)
        {
            text += "\n " + GetDescriptionOfRoom1();
        }
        DescriptionText.text = text;
    }

    private string GetDescriptionOfRoom1()
    {
        return
            "´Go around in the room and when you are comfortable with the technique open the door to go to the next room";
    }

}
