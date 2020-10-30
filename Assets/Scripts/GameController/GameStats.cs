using System;
using UnityEditor;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static int WavesKilled = 0;
    
    public static int EnemiesSpawned = 0;
    public static int EnemiesKilled = 0;

    [SerializeField] private bool _isPlayerAlive = false;

    //components that hold Action events
    private PlayerController _playerController;
    
    private void Start()
    {
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
        
        //maybe you can create a enemy birth and death listener
        //by calling a function call/event here - from EnemySpawner.cs
        //for eg: void InformAboutEnemy(GameObject enemy)
        //enemy.getcomponent enemycontroller().xyz subscribe
        
        //store all of these enemies in a list and remove them from the list on their death also
    }

    private void OnDestroy()
    {
        
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
