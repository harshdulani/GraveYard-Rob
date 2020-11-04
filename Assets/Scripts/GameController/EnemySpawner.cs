using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemyPrefabs;

    //you may write a dynamic "get all spawnpoints at runtime" function in Start or Awake
    //but that would slow down stuff
    //so for now, i'm just holding all spawn points in a heirarchy
    //and dragging all of those to the editor drop point but without opening the array details
    [SerializeField]
    private Transform[] spawnPoints;

    private void Start()
    {
        //print(enemyPrefabs.Length + " enemyPrefabs in dictionary, to be spawned at " + spawnPoints.Length + " locations.");
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0)
            Debug.Break();
    }

    public void SpawnNewEnemy()
    {
        var enemyInstance = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length - 1)],
            spawnPoints[Random.Range(0, spawnPoints.Length)]).transform;
        //if you don't parent it to null, it gets parented to this game obj
        enemyInstance.SetParent(null);
        
        EnemyEvents.current.InvokeEnemyBirth(enemyInstance.transform);
    }
}
