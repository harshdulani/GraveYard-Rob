using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelFlowController : MonoBehaviour
{
    public List<GameObject> thingsToEnableWhenGameplayStarts;

    public CinemachineVirtualCamera climbDownFenceCamera;
    public CinemachineFreeLook tpsCamera;
    
    [Header("Enemy Spawning")]
    public float gameplayStartWaitTime;

    [Header("Dialogues")] 
    public List<string> enemySpawnStartDialogues;
    public Text dialogueText;
    
    [Header("Screenshake at first enemy birth")] 
    public int screenShakeIntensity = 30;
    public float screenShakeSustainTime = 1.5f;

    [Header("Final Objective")] 
    public GameObject van;
    public GameObject gateClosed, gateOpen;
    
    public CinemachineVirtualCamera stareAtEnemy;
    public CinemachineTargetGroup targetGroup;

    private EnemyWaveController _waveController;

    private WaitForSeconds _waitScreenShake;
    private WaitForEndOfFrame _endOfFrame;
    
    private void OnEnable()
    {
        GameFlowEvents.current.gameplayStart += OnGameplayStart;
        GameFlowEvents.current.gameplayPause += OnGameplayPause;
        GameFlowEvents.current.gameplayResume += OnGameplayResume;
        GameFlowEvents.current.updateObjective += OnUpdateObjective;

        PlayerEvents.current.playerDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        GameFlowEvents.current.gameplayStart -= OnGameplayStart;
        GameFlowEvents.current.gameplayPause -= OnGameplayPause;
        GameFlowEvents.current.gameplayResume -= OnGameplayResume;
        GameFlowEvents.current.updateObjective -= OnUpdateObjective;
        
        PlayerEvents.current.playerDeath -= OnPlayerDeath;
    }

    private void Start()
    {
        _waveController = GetComponent<EnemyWaveController>();
        
        _waitScreenShake = new WaitForSeconds(screenShakeSustainTime);
        _endOfFrame = new WaitForEndOfFrame();
    }
    
    private void OnGameplayStart()
    {
        //_waveController.StartWaveSpawning(gameplayStartWaitTime);

        foreach (var thing in thingsToEnableWhenGameplayStarts)
        {
            thing.SetActive(true);
        }
    }

    private void OnGameplayPause()
    {
        tpsCamera.gameObject.SetActive(false);
        climbDownFenceCamera.gameObject.SetActive(false);
        MovementInput.current.TakeAwayMovementControl();
    }

    private void OnGameplayResume()
    {
        tpsCamera.gameObject.SetActive(true);
        MovementInput.current.GiveBackMovementControl();
    }

    private void OnUpdateObjective()
    {
        if (GameStats.current.currentObjective == 1)
        {
            ShowDialogue();
            StartCoroutine(WaitTillYouFindEnemy());
        }
        else if (GameStats.current.currentObjective == 4)
        {
            van.SetActive(true);
            gateOpen.SetActive(true);
            gateClosed.SetActive(false);
        }
    }

    private IEnumerator WaitTillYouFindEnemy()
    {
        Time.timeScale = 0.75f;
        while (GameStats.current.activeEnemies.Count == 0)
            yield return _endOfFrame;
        
        GameStats.current.activeEnemies[0].GetComponent<ScreenShakes>().CustomShake(screenShakeIntensity, screenShakeSustainTime);

        yield return _waitScreenShake;
        Time.timeScale = 1f;
    }

    private void ShowDialogue()
    {
        if(!dialogueText) return;
        
        dialogueText.transform.parent.parent.gameObject.SetActive(true);
        if (GameStats.current.currentObjective == 1)
            dialogueText.text = enemySpawnStartDialogues[Random.Range(0, enemySpawnStartDialogues.Count - 1)];
        
        Destroy(dialogueText.transform.parent.parent.gameObject, 3f);
    }

    private IEnumerator LookAtEnemy()
    {
        MovementInput.current.TakeAwayMovementControl();

        while (GameStats.current.activeEnemies.Count == 0)
        {
            yield return null;
        }
        
        stareAtEnemy.gameObject.SetActive(true);
        
        targetGroup.AddMember(GameStats.current.activeEnemies[0], 1f, 1.5f);

        yield return new WaitForSecondsRealtime(2f);
        
        while (Time.timeScale <= 1f)
        {
            Time.timeScale += 0.05f;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        
        Destroy(stareAtEnemy.gameObject);
        
        targetGroup.RemoveMember(GameStats.current.activeEnemies[0]);
        
        MovementInput.current.GiveBackMovementControl();
    }
    
    private void OnPlayerDeath()
    {
        StopAllCoroutines();
        _waveController.EndWaveSpawning();
    }
}