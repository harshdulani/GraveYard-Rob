using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelFlowController : MonoBehaviour
{
    [Header("General")]
    public bool shouldSpawn = true;
    public float spawnStartWaitTime;

    [Header("Enemy Spawning")] 
    public Text enemyCountText;
    public string enemyCountPrefix;
    
    public int idealEnemyCount;
    public int deviationEnemyCount;
    public float idealWaitBeforeSpawning, deviationWaitBeforeSpawning;
    public int currentEnemyCount;

    [Header("Waves")] 
    public Text waveCountText;    
    public string waveCountPrefix;
    
    public float idealBreakTimeBetweenWaves;
    public float deviationBreakTimeBetweenWaves;
    public int idealWaveCount, deviationWaveCount;
    public int currentWaveCount;

    private bool _isSpawnerRunning;
    
    private EnemySpawner _spawner;

    private void Start()
    {
        _spawner = GetComponent<EnemySpawner>();

        _isSpawnerRunning = shouldSpawn;

        UpdateWaveCount();
        UpdateEnemyCount();
        
        //this will ofc be called at another place where it makes sense
        //too late, forgot what i meant by making more sense here^
        if(_isSpawnerRunning)
            StartCoroutine(SpawnLoop());
    }

    private void UpdateWaveCount(int totalWaves = 0)
    {
        waveCountText.text = waveCountPrefix + currentWaveCount + " / " + totalWaves;
    }

    private void UpdateEnemyCount(int totalEnemies = 0)
    {
        //change this to enemies killed
        enemyCountText.text = enemyCountPrefix + currentEnemyCount + " / " + totalEnemies;
    }
    
    private IEnumerator SpawnLoop()
    {
        while (_isSpawnerRunning)
        {
            //wait before starting to spawn enemies
            yield return new WaitForSeconds(spawnStartWaitTime);

            //calculate waves in current game
            int wavesInThisGame = Random.Range(idealWaveCount - deviationWaveCount,
                idealWaveCount + deviationWaveCount);

            for (currentWaveCount = 0; currentWaveCount < wavesInThisGame; currentWaveCount++)
            {
                UpdateWaveCount(wavesInThisGame);
                
                //calculate enemies in current wave
                int enemiesInThisWave = Random.Range(idealEnemyCount - deviationEnemyCount,
                    idealEnemyCount + deviationEnemyCount);
                
                UpdateEnemyCount(enemiesInThisWave);

                for (currentEnemyCount = 0; currentEnemyCount < enemiesInThisWave; currentEnemyCount++)
                {
                    if (shouldSpawn)
                    {
                        _spawner.SpawnNewEnemy();
                        UpdateEnemyCount(enemiesInThisWave);
                    }

                    yield return new WaitForSeconds(
                        Random.Range(idealWaitBeforeSpawning - deviationWaitBeforeSpawning,
                            idealWaitBeforeSpawning + deviationWaitBeforeSpawning));
                    
                    //enemy is spawned here
                }
                
                //wave ends here
                yield return new WaitForSeconds(Random.Range(
                    idealBreakTimeBetweenWaves - deviationBreakTimeBetweenWaves,
                    idealBreakTimeBetweenWaves + deviationBreakTimeBetweenWaves));
            }
        }
    }
}