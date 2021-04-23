using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public List<float> waitBeforeSpawningEnemy;
    public int currentEnemiesSpawnedCount;
    public int enemiesInThisWave, enemiesKilledInThisWave;

    [Header("Waves")]
    public Text waveCountText;    
    public string waveCountPrefix;

    public bool shouldSpawnWaveAtOnce;
    public int[] minimumEnemyCountByType;

    public List<float> breakTimeBetweenWaves;
    public int idealWaveCount, deviationWaveCount;
    public int currentWaveCount;

    private bool _hasWaveEnded;
    
    private EnemySpawner _spawner;
    private List<WaitForSeconds> _waitSpawningEnemy, _waitWaveBreaktime;
    private WaitForSeconds _waitOneSec, _waitCountdownTextUpdate;

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

        _waitSpawningEnemy = new List<WaitForSeconds>();
        _waitWaveBreaktime = new List<WaitForSeconds>();
        _waitOneSec = new WaitForSeconds(1f);
        _waitCountdownTextUpdate = new WaitForSeconds(0.01f);
        
        foreach (var waitTime in waitBeforeSpawningEnemy)
            _waitSpawningEnemy.Add(new WaitForSeconds(waitTime));

        foreach (var waitTime in breakTimeBetweenWaves)
            _waitWaveBreaktime.Add(new WaitForSeconds(waitTime));
    }

    private IEnumerator SpawnLoop(float startWaitTime = 0f)
    {
        while (isSpawnerRunning)
        {
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
                    enemiesKilledInThisWave = 0;
                    
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

                        yield return _waitSpawningEnemy[Random.Range(0, _waitSpawningEnemy.Count)];
                    }
                }

                while(enemiesInThisWave - enemiesKilledInThisWave > 1)
                {
                    yield return _waitOneSec;
                }
                
                //wave ends here
                var waitIndex = Random.Range(0, breakTimeBetweenWaves.Count);

                StartCoroutine(CountDown(breakTimeBetweenWaves[waitIndex]));
                yield return _waitWaveBreaktime[waitIndex];
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
                waveCountText.text = (endTime - currentTime).ToString("0.00");
                yield return _waitCountdownTextUpdate;
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

    private void StartWaveSpawning()
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
