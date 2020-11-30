using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesCanvasController : MonoBehaviour
{
    public List<string> objectiveTexts;

    [Header("UI elements")] public Text objectiveTitle;
    
    private int _currentObjective;

    private void OnEnable()
    {
        GameFlowEvents.current.updateObjective += UpdateObjective;
    }

    private void OnDisable()
    {
        GameFlowEvents.current.updateObjective -= UpdateObjective;
    }

    private void Start()
    {
        objectiveTitle.text = objectiveTexts[_currentObjective];
    }

    private void UpdateObjective()
    {
        if(_currentObjective < objectiveTexts.Count)
            objectiveTitle.text = objectiveTexts[++_currentObjective];
    }
}