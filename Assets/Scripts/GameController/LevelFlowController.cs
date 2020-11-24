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