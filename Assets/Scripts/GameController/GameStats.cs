using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats current;
    
    public List<Transform> activeEnemies;
    
    public int EnemiesAlive => activeEnemies.Count;

    //might be useful for a respawn ability, game start checks, game end checks  
    public bool isPlayerAlive, isGamePlaying;

    public int currentObjective;

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
        activeEnemies = new List<Transform>();
    }

    private void OnEnemyBirth(Transform enemy)
    {
        activeEnemies.Add(enemy);
    }

    private void OnEnemyDeath(Transform enemy)
    {
        activeEnemies.Remove(enemy);
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
