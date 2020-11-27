using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFlowController : MonoBehaviour
{
    public List<GameObject> thingsToEnableWhenGameplayStarts;

    public CinemachineVirtualCamera climbDownFenceCamera;
    public CinemachineFreeLook tpsCamera;
    
    [Header("Enemy Spawning")]
    public float gameplayStartWaitTime;

    private EnemyWaveController _waveController; 
    
    private void OnEnable()
    {
        GameFlowEvents.current.gameplayStart += OnGameplayStart;

        PlayerEvents.current.playerDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        GameFlowEvents.current.gameplayStart -= OnGameplayStart;
        
        PlayerEvents.current.playerDeath -= OnPlayerDeath;
    }

    private void Start()
    {
        _waveController = GetComponent<EnemyWaveController>();

        //TODO: add a black screen w some text/logo that clears up 1 seconds after this start is executed so that all starts are executed
        //and players don't have to see a stutter
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            GameFlowEvents.current.InvokeGameplayPause();
            OnGameplayPause();
        }
        
    }

    private void OnGameplayStart()
    {
        _waveController.StartWaveSpawning(gameplayStartWaitTime);

        foreach (var thing in thingsToEnableWhenGameplayStarts)
        {
            thing.SetActive(true);
        }
    }

    private void OnGameplayPause()
    {
        Time.timeScale = 0.01f;
        
        var loading = SceneManager.LoadSceneAsync("PauseMenuScene", LoadSceneMode.Additive);
        loading.completed += OnPauseMenuLoading;
    }

    
    private void OnPauseMenuLoading(AsyncOperation operation)
    {
        tpsCamera.gameObject.SetActive(false);
        climbDownFenceCamera.gameObject.SetActive(false);
        operation.completed -= OnPauseMenuLoading;
    }

    private void OnReturnFromPause()
    {
        
    }
    
    private void OnPlayerDeath()
    {
        StopAllCoroutines();
        _waveController.EndWaveSpawning();
    }
}