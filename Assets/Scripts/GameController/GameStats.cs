using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats current;
    
    private static List<Transform> _activeEnemies; 
    public static int WavesKilled = 0;

    private static int _enemiesSpawned = 0;
    public static int EnemiesKilled = 0;

    //might be useful for a respawn ability, game start checks, game end checks  
    [SerializeField] private bool _isPlayerAlive = false;

    //components that hold Action events
    private PlayerController _playerController;

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
    }

    private void OnDisable()
    {
        EnemyEvents.current.enemyBirth -= OnEnemyBirth;
        EnemyEvents.current.enemyDeath -= OnEnemyDeath;
    }

    private void Start()
    {
        _activeEnemies = new List<Transform>();
        //subscribe to:
        /*
         * player birth
         * player death
         *
         * enemy birth
         * enemy death
         *    *create function to check for each kill -> whether this kill ended a wave
         *    *you can achieve, for eg: behaviour for after game ends. spawning mini enemies
         *     after waves end for time pass i guess idk
         *     whatever it will be useful
         *
         * wave start
         */

        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _playerController.PlayerBirth += OnPlayerBirth;
        _playerController.PlayerDeath += OnPlayerDeath;
        
        //by calling a function call/event here - from EnemySpawner
        
        //store all of these enemies in a list and remove them from the list on their death also
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
        _isPlayerAlive = true;
    }

    private void OnPlayerDeath()
    {
        _isPlayerAlive = false;
    }
}
