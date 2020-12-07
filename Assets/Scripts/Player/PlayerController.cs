using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerController : MonoBehaviour
{
    [Header("Stamina")] 
    public int autoHealStaminaPerSecond = 5;
    public float timeBeforeStaminaHeal = 3.5f;
    private float _elapsedTimeBeforeStaminaHeal = 0f;
    
    [Header("Cameras")]
    public CinemachineVirtualCamera climbDownFenceCamera;
    public CinemachineFreeLook tpsCamera;

    private PlayerWeaponController _weaponController;
    
    private static readonly int PlayerBorn = Animator.StringToHash("playerBorn");

    private void Start()
    {
        _weaponController = GetComponentInChildren<PlayerWeaponController>();
        _weaponController.gameObject.SetActive(false);
        PlayerEvents.current.InvokePlayerBirth();

        //so that hes not visible on main menu
        transform.localScale = new Vector3(0, 0, transform.localScale.z);
    }

    private void OnEnable()
    {
        GameFlowEvents.current.gameplayStart += OnGameplayStart;
        PlayerEvents.current.playerDeath += OnPlayerDeath;

        PlayerEvents.current.endCombatStrike += OnStaminaUse;
    }

    private void OnDisable()
    {
        GameFlowEvents.current.gameplayStart -= OnGameplayStart;
        PlayerEvents.current.playerDeath -= OnPlayerDeath;
        
        PlayerEvents.current.endCombatStrike -= OnStaminaUse;
    }

    private void OnGameplayStart()
    {
        transform.localScale = Vector3.one;
        GetComponent<Animator>().SetBool(PlayerBorn, true);
    }

    public void OnClimbDownFence()
    {
        GetComponent<PlayerCombat>().SwapWeapon();
        //so that attacks can happen
        GameStats.current.isGamePlaying = true;

        climbDownFenceCamera.gameObject.SetActive(false);
        tpsCamera.gameObject.SetActive(true);
    }

    private void OnPlayerDeath()
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (PlayerStats.main.playerStamina >= PlayerStats.main.maxStamina) return;

        if (_elapsedTimeBeforeStaminaHeal <= timeBeforeStaminaHeal)
            _elapsedTimeBeforeStaminaHeal += Time.fixedDeltaTime;
        else
        {
            PlayerStats.main.OnStaminaChange(-(autoHealStaminaPerSecond / 5));
        }
    }

    public void OnStaminaUse()
    {
        _elapsedTimeBeforeStaminaHeal = 0f;
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.CompareTag("Projectile"))
        {
            PlayerEvents.current.healthChange(-hit.gameObject.GetComponent<ProjectileController>().projectileDamage);
            print("projectile hit w " + hit.gameObject.name);
            Destroy(hit.gameObject);
        }
    }
}