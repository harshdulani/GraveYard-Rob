using System;
using UnityEditor;
using UnityEngine;

public class GameFlowEvents : MonoBehaviour
{
    public static GameFlowEvents current;

    public Action gameplayStart, gameplayPause, gameplayResume, gameplayEnd;

    public Action updateObjective;
    
    private void Awake()
    {
        if (current == null)
            current = this;
        else
            Destroy(this);
    }

    public void InvokeGameplayStart()
    {
        gameplayStart?.Invoke();
    }

    public void InvokeGameplayEnd()
    {
        gameplayEnd?.Invoke();
    }

    public void InvokeGameplayPause()
    {
        gameplayPause?.Invoke();
    }
    
    public void InvokeGameplayResume()
    {
        gameplayResume?.Invoke();
    }

    public void InvokeUpdateObjective()
    {
        updateObjective?.Invoke();
    }
}
