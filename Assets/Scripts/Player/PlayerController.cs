using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stamina Healing")] 
    public int autoHealStaminaPerSecond = 5;
    public float timeBeforeHealing = 3.5f;
    
    private float _elapsedTimeBeforeStaminaHeal = 0f;

    [Header("Health Healing")] 
    public int autoHealHealthPerSecond = 5;
    
    private float _elapsedTimeBeforeHealthHeal = 0f;
    
    [Header("Cameras")]
    public CinemachineVirtualCamera climbDownFenceCamera, deathCamera;
    public CinemachineFreeLook tpsCamera;

    public float onDeathTimeScale = 0.5f;
    
    [Header("Audio")] public List<AudioClip> hits;
    public AudioClip hitHeavy;

    public float timeBeforeHitSound = 0.2f;

    private float _elapsedTimeBeforeHitSound;
    private bool _isWaitingToMakeHitSound;
    private AudioSource _audioSource;
    
    private PlayerWeaponController _weaponController;
    private Animator _animator;
    private ScreenShakes _shakes;
    
    private static readonly int PlayerBorn = Animator.StringToHash("playerBorn");
    private static readonly int PlayerDeath = Animator.StringToHash("playerDeath");
    private static readonly int PlayerTakeHit = Animator.StringToHash("playerTakeHit");

    private void OnEnable()
    {
        GameFlowEvents.current.gameplayStart += OnGameplayStart;
        PlayerEvents.current.playerDeath += OnPlayerDeath;

        PlayerEvents.current.endCombatStrike += ResetStaminaHealTimer;
        PlayerEvents.current.startCombatStrike += ResetHealthHealTimer;
    }

    private void OnDisable()
    {
        GameFlowEvents.current.gameplayStart -= OnGameplayStart;
        PlayerEvents.current.playerDeath -= OnPlayerDeath;
        
        PlayerEvents.current.endCombatStrike -= ResetStaminaHealTimer;
        PlayerEvents.current.startCombatStrike -= ResetHealthHealTimer;
    }
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _shakes = GetComponent<ScreenShakes>();
        _audioSource = GetComponent<AudioSource>();
        
        GetComponentInChildren<PlayerWeaponController>().gameObject.SetActive(false);

        PlayerEvents.current.InvokePlayerBirth();

        //so that hes not visible on main menu
        transform.localScale = new Vector3(0, 0, transform.localScale.z);
    }

    private void FixedUpdate()
    {
        if(!GameStats.current.isPlayerAlive) return;
        
        //hit sound countdown
        if (_isWaitingToMakeHitSound)
        {
            print(_elapsedTimeBeforeHitSound + " <= " + timeBeforeHitSound);
            if (_elapsedTimeBeforeHitSound <= timeBeforeHitSound)
                _elapsedTimeBeforeHitSound += Time.fixedDeltaTime;
            else
            {
                _isWaitingToMakeHitSound = false;
            }
        }
        
        //stamina regen
        if (PlayerStats.main.playerStamina < PlayerStats.main.maxStamina)
        {
            if (_elapsedTimeBeforeStaminaHeal <= timeBeforeHealing)
                _elapsedTimeBeforeStaminaHeal += Time.fixedDeltaTime;
            else
            {
                PlayerEvents.current.InvokeStaminaChange(-(autoHealStaminaPerSecond / 50));
            }
        }

        //health regen
        if (PlayerStats.main.playerHealth < PlayerStats.main.maxHealth)
        {
            if (_elapsedTimeBeforeHealthHeal <= timeBeforeHealing)
                _elapsedTimeBeforeHealthHeal += Time.fixedDeltaTime;
            else
            {
                PlayerEvents.current.InvokeHealthChange(-Mathf.CeilToInt(autoHealHealthPerSecond / 50f), AttackType.Heal);
            }
        }
    }
    
    private void OnGameplayStart()
    {
        transform.localScale = Vector3.one;
        _animator.SetBool(PlayerBorn, true);
    }

    public void OnClimbDownFence()
    {
        _shakes.Heavy();
        GetComponent<PlayerCombat>().SwapWeapon();
        //so that all the linked things can start happening
        GameStats.current.isGamePlaying = true;

        climbDownFenceCamera.gameObject.SetActive(false);
        tpsCamera.gameObject.SetActive(true);
    }

    public void OnPlayerTakeHit()
    {
        //to prevent getting hit after death
        if (!GameStats.current.isPlayerAlive) return;
        
        _shakes.Light();
        ResetHealthHealTimer();
        _animator.SetTrigger(PlayerTakeHit);

        
        _audioSource.PlayOneShot(hits[Random.Range(0, hits.Count)], 0.65f);
    }

    public void OnPlayerTakeBump()
    {
        ResetHealthHealTimer();
        
        if (_isWaitingToMakeHitSound) return;
        
        print(_isWaitingToMakeHitSound + " making hit");
        _audioSource.PlayOneShot(hits[Random.Range(0, hits.Count)], 0.65f);
        _isWaitingToMakeHitSound = true;
        _elapsedTimeBeforeHitSound = 0f;
    }
    
    private void OnPlayerDeath()
    {
        if(!GameStats.current.isPlayerAlive) return;

        _animator.SetTrigger(PlayerDeath);
        _audioSource.PlayOneShot(hitHeavy);
        
        GameStats.current.isPlayerAlive = false;
        MovementInput.current.TakeAwayMovementControl();
        Time.timeScale = onDeathTimeScale;
        tpsCamera.gameObject.SetActive(false);
        deathCamera.gameObject.SetActive(true);
    }

    public void ResetStaminaHealTimer()
    {
        //the stamina regen timer is reset every time you use stamina
        _elapsedTimeBeforeStaminaHeal = 0f;
    }

    private void ResetHealthHealTimer()
    {
        //the health regen timer is to be reset every time you enter combat
            //every time you attack someone
            //every time someone attacks you
            
        _elapsedTimeBeforeHealthHeal = 0f;
    }
}