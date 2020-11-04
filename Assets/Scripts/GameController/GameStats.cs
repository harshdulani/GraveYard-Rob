using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameStats : MonoBehaviour
{
    public static GameStats current;
    
    private static List<Transform> _activeEnemies; 
    public static int WavesKilled = 0;

    private static int _enemiesSpawned = 0;
    public static int EnemiesKilled = 0;

    //might be useful for a respawn ability, game start checks, game end checks  
    [SerializeField] private bool isPlayerAlive = false;

    private void Awake()
    {
        if (current == null)
            current = this;
        else
            Destroy(current);
    }

    private void OnEnable()
    {
        EnemyEvents.current.enemyBirth += OnEnemyBirth;
        EnemyEvents.current.enemyDeath += OnEnemyDeath;

        PlayerEvents.current.playerBirth += OnPlayerBirth;
        PlayerEvents.current.playerDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        EnemyEvents.current.enemyBirth -= OnEnemyBirth;
        EnemyEvents.current.enemyDeath -= OnEnemyDeath;
        
        PlayerEvents.current.playerBirth -= OnPlayerBirth;
        PlayerEvents.current.playerDeath -= OnPlayerDeath;
    }

    private void Start()
    {
        _activeEnemies = new List<Transform>();
    }

    private void OnEnemyBirth(Transform enemy)
    {
        _activeEnemies.Add(enemy);
        _enemiesSpawned++;
    }

    private void OnEnemyDeath(Transform enemy)
    {
        _activeEnemies.Remove(enemy);
        EnemiesKilled++;
    }
    
    private void OnPlayerBirth()
    {
        isPlayerAlive = true;
    }

    private void OnPlayerDeath()
    {
        isPlayerAlive = false;
    }
}
