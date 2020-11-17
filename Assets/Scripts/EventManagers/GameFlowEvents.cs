using System;
using UnityEditor;
using UnityEngine;

public class GameFlowEvents : MonoBehaviour
{
    public static GameFlowEvents current;

    public Action gameplayStart, gameplayEnd;
    
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
}
