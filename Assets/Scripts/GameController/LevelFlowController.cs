using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LevelFlowController : MonoBehaviour
{
    public List<GameObject> thingsToEnableWhenGameplayStarts;
    
    [Header("Enemy Spawning")]
    public float gameplayStartWaitTime;

    private EnemyWaveController _waveController; 
    
    private void OnEnable()
    {
        //EnemyEvents.current.enemyDeath += OnEnemyDeath;
        GameFlowEvents.current.gameplayStart += OnGameplayStart;

        PlayerEvents.current.playerDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        //EnemyEvents.current.enemyDeath -= OnEnemyDeath;
        GameFlowEvents.current.gameplayStart -= OnGameplayStart;
        
        PlayerEvents.current.playerDeath -= OnPlayerDeath;
    }

    private void Start()
    {
        _waveController = GetComponent<EnemyWaveController>();
        
        //TODO: add a black screen w some text/logo that clears up 1 seconds after this start is executed so that all starts are executed
        //and players dont have to see a stutter
    }

    private void OnGameplayStart()
    {
        _waveController.StartWaveSpawning(gameplayStartWaitTime);

        //change CM Live camera field
        foreach (var thing in thingsToEnableWhenGameplayStarts)
        {
            thing.SetActive(true);
        }
    }
    
    private void OnPlayerDeath()
    {
        StopAllCoroutines();
        _waveController.EndWaveSpawning();
    }
}