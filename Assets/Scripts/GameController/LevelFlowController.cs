using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFlowController : MonoBehaviour
{
    [Header("Enemy Spawning")]
    public bool shouldSpawn = true;
    public float minWaitTime, maxWaitTime;

    private EnemySpawner spawner;

    private void Start()
    {
        spawner = GetComponent<EnemySpawner>();
        //this will ofc be called at another place where it makes sense
        if(shouldSpawn)
            StartCoroutine("SpawnLoop");
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (shouldSpawn)
                spawner.SpawnNewEnemy();

            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        }
    }
}
