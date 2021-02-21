using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemyPrefabs;
    //i'm just holding all spawn points in a heirarchy
    //and dragging all of those to the editor drop point but without opening the array details
    [SerializeField]
    private Transform[] spawnPoints;
    
    public AudioSource audioSource;
    
    private void Start()
    {
        //print(enemyPrefabs.Length + " enemyPrefabs in dictionary, to be spawned at " + spawnPoints.Length + " locations.");
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0)
            Debug.Break();
    }

    public void SpawnNewEnemy()
    {
        var enemyInstance = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)],
            spawnPoints[Random.Range(0, spawnPoints.Length)]).transform;
        //if you don't parent it to null, it gets parented to this game obj
        enemyInstance.SetParent(null);
        
        EnemyEvents.current.InvokeEnemyBirth(enemyInstance.transform);
    }

    private int SpawnEnemyCustom(int enemyType, List<Transform> newSpawnPointsList)
    {
        var spawnPointIndex = Random.Range(0, newSpawnPointsList.Count);
        
        var enemyInstance = Instantiate(enemyPrefabs[enemyType], newSpawnPointsList[spawnPointIndex]).transform;
        
        enemyInstance.SetParent(null);
        
        EnemyEvents.current.InvokeEnemyBirth(enemyInstance.transform);
        
        return spawnPointIndex;
    }

    public void SpawnNewWave(int ghosts, int demons)
    {
        var tempList = spawnPoints.ToList();
        
        for (int i = 0; i < ghosts; i++)
        {
            tempList.RemoveAt(SpawnEnemyCustom(0, tempList));
        }

        for (int i = 0; i < demons; i++)
        {
            tempList.RemoveAt(SpawnEnemyCustom(1, tempList));
        }
        audioSource.Play();
    }
}
