using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class EnemyWaveController : MonoBehaviour
{
    [Header("General")] 
    public bool isSpawnerRunning;

    [Header("Enemy Spawning")] 
    public Text enemyCountText;
    public string enemyCountPrefix;
    
    public int idealEnemyCount;
    public int deviationEnemyCount;
    public float idealWaitBeforeSpawning, deviationWaitBeforeSpawning;
    public int currentEnemiesSpawnedCount;
    public int enemiesInThisWave, enemiesKilledInThisWave;

    [Header("Waves")]
    public Text waveCountText;    
    public string waveCountPrefix;

    public bool shouldSpawnWaveAtOnce;
    public int[] minimumEnemyCountByType;

    public float idealBreakTimeBetweenWaves;
    public float deviationBreakTimeBetweenWaves;
    public int idealWaveCount, deviationWaveCount;
    public int currentWaveCount;

    private bool _hasWaveEnded;
    
    private EnemySpawner _spawner;

    private void OnEnable()
    {
        EnemyEvents.current.enemyDeath += OnEnemyDeath;
        GameFlowEvents.current.updateObjective += StartWaveSpawning;
    }

    private void OnDisable()
    {
        EnemyEvents.current.enemyDeath -= OnEnemyDeath;
    }

    private void Start()
    {
        _spawner = GetComponent<EnemySpawner>();
    }

    private IEnumerator SpawnLoop(float startWaitTime = 0f)
    {
        while (isSpawnerRunning)
        {
            //StartCoroutine(CountDown(startWaitTime));
            //wait before starting to spawn enemies
            //yield return new WaitForSeconds(startWaitTime);

            //calculate waves in current game
            int wavesInThisGame = Random.Range(idealWaveCount - deviationWaveCount,
                idealWaveCount + deviationWaveCount);

            for (currentWaveCount = 0; currentWaveCount < wavesInThisGame; currentWaveCount++)
            {
                UpdateWaveCount(wavesInThisGame);
                
                
                if (shouldSpawnWaveAtOnce)
                {
                    var ghosts = Random.Range(minimumEnemyCountByType[0],
                        minimumEnemyCountByType[0] + deviationEnemyCount);

                    var demons = Random.Range(minimumEnemyCountByType[1],
                            minimumEnemyCountByType[1] + deviationEnemyCount);

                    enemiesInThisWave = ghosts + demons;
                    
                    _spawner.SpawnNewWave(ghosts, demons);
                    UpdateEnemyCount();
                }
                else
                {
                    //calculate enemies in current wave
                    enemiesInThisWave = Random.Range(idealEnemyCount - deviationEnemyCount,
                        idealEnemyCount + deviationEnemyCount);
                
                    UpdateEnemyCount();

                    for (currentEnemiesSpawnedCount = 0;
                        currentEnemiesSpawnedCount < enemiesInThisWave;
                        currentEnemiesSpawnedCount++)
                    {
                        //spawn these enemies
                        _spawner.SpawnNewEnemy();
                        UpdateEnemyCount();

                        yield return new WaitForSeconds(
                            Random.Range(idealWaitBeforeSpawning - deviationWaitBeforeSpawning,
                                idealWaitBeforeSpawning + deviationWaitBeforeSpawning));
                    }
                }

                while(enemiesKilledInThisWave != enemiesInThisWave)
                    yield return new WaitForSeconds(1f);
                
                //wave ends here
                var waitTime = Mathf.Ceil(Random.Range(
                    idealBreakTimeBetweenWaves - deviationBreakTimeBetweenWaves,
                    idealBreakTimeBetweenWaves + deviationBreakTimeBetweenWaves));
                
                print(waitTime);
                
                StartCoroutine(CountDown(waitTime));
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    private void UpdateWaveCount(int totalWaves = 0)
    {
        //this needs to move out of here
        waveCountText.text = waveCountPrefix + currentWaveCount + " / " + totalWaves;
    }

    private void UpdateEnemyCount()
    {
        //this needs to move out of here
        //change this to enemies killed
        enemyCountText.text = enemyCountPrefix + enemiesKilledInThisWave + " / " + enemiesInThisWave;
    }

    private IEnumerator CountDown(float seconds)
    {
        var startTime = Time.time;
        var endTime = startTime + seconds;
        
        while (true)
        {
            var currentTime = Time.time;
            if (currentTime < endTime)
            {
                waveCountText.text = (endTime - currentTime).ToString("0.0");
                yield return new WaitForSeconds(0.01f);
            }
            else
                break;
        }
    }

    private void OnEnemyDeath(Transform enemy)
    {
        //this marks the end of the wave and hence the start of a wait period
        enemiesKilledInThisWave++;
        UpdateEnemyCount();
    }

    public void StartWaveSpawning()
    {
        UpdateWaveCount();
        UpdateEnemyCount();
        
        GameFlowEvents.current.updateObjective -= StartWaveSpawning;
        
        if(isSpawnerRunning)
            StartCoroutine(SpawnLoop());
    }
    
    public void EndWaveSpawning()
    {
        StopAllCoroutines();
    }
}
