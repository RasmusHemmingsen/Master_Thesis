using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionManager : MonoBehaviour
{

    public Text m_Text;

    public void SwitchDescription(LocomotionManager.LocomotionTechinique techinique)
    {
        switch (techinique)
        {
            case LocomotionManager.LocomotionTechinique.SmoothLocomotion:
                SetToSmoothLocomotionDescription();
                break;
            case LocomotionManager.LocomotionTechinique.Teleport:
                SetToTeleportDescription();
                break;
            case LocomotionManager.LocomotionTechinique.DashStep:
                SetToDashStepDescription();
                break;
            case LocomotionManager.LocomotionTechinique.Armswing:
                SetToArmswingDescription();
                break;
            case LocomotionManager.LocomotionTechinique.Cybershoes:
                SetToCybershoesDescription();
                break;
            case LocomotionManager.LocomotionTechinique.BlinkStep:
                SetToBlinkStepDescription();
                break;
            default:
                m_Text.text = "";
                break;
        }
    }

    private void SetToTeleportDescription()
    {
        m_Text.text = "Teleport \n";
    }

    private void SetToDashStepDescription()
    {
        m_Text.text = "Dash Step \n";
    }

    private void SetToBlinkStepDescription()
    {
        m_Text.text = "Blink Step \n";
    }

    private void SetToArmswingDescription()
    {
        m_Text.text = "Armswing \n";
    }

    private void SetToCybershoesDescription()
    {
        m_Text.text = "Cybershoes \n";

    }

    private void SetToSmoothLocomotionDescription()
    {
        m_Text.text = "Smooth locomotion \n";
    }

}
