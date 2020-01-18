using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultRoomManager : MonoBehaviour
{
    [SerializeField]
    private DescriptionManager _descriptionManager;

    private void Start()
    {
        _descriptionManager.SwitchDescription(ExperimentManager.ExperimentManagerVariable.CurrentLocomotionTechnique, true);
    }
}
