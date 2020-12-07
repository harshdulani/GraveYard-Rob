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
    public CinemachineVirtualCamera climbDownFenceCamera;
    public CinemachineFreeLook tpsCamera;

    private PlayerWeaponController _weaponController;
    private Animator _animator;
    
    private static readonly int PlayerBorn = Animator.StringToHash("playerBorn");
    private static readonly int PlayerDeath = Animator.StringToHash("playerDeath");
    private static readonly int PlayerTakeHit = Animator.StringToHash("playerTakeHit");

    private void Start()
    {
        _weaponController = GetComponentInChildren<PlayerWeaponController>();
        _animator = GetComponent<Animator>();
        
        _weaponController.gameObject.SetActive(false);
        PlayerEvents.current.InvokePlayerBirth();

        //so that hes not visible on main menu
        transform.localScale = new Vector3(0, 0, transform.localScale.z);
    }

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

    private void OnGameplayStart()
    {
        transform.localScale = Vector3.one;
        _animator.SetBool(PlayerBorn, true);
    }

    public void OnClimbDownFence()
    {
        GetComponent<PlayerCombat>().SwapWeapon();
        //so that attacks can happen
        GameStats.current.isGamePlaying = true;

        climbDownFenceCamera.gameObject.SetActive(false);
        tpsCamera.gameObject.SetActive(true);
    }

    public void OnPlayerTakeHit()
    {
        ResetHealthHealTimer();
        _animator.SetTrigger(PlayerTakeHit);
    }
    
    private void OnPlayerDeath()
    {
        _animator.SetTrigger(PlayerDeath);
        MovementInput.current.TakeAwayMovementControl();
        //Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        //stamina regen
        if (PlayerStats.main.playerStamina < PlayerStats.main.maxStamina)
        {
            if (_elapsedTimeBeforeStaminaHeal <= timeBeforeHealing)
                _elapsedTimeBeforeStaminaHeal += Time.fixedDeltaTime;
            else
            {
                PlayerStats.main.OnStaminaChange(-(autoHealStaminaPerSecond / 50));
            }
        }

        //health regen
        if (PlayerStats.main.playerHealth < PlayerStats.main.maxHealth)
        {
            if (_elapsedTimeBeforeHealthHeal <= timeBeforeHealing)
                _elapsedTimeBeforeHealthHeal += Time.fixedDeltaTime;
            else
            {
                PlayerStats.main.OnHealthChange(-(autoHealHealthPerSecond / 50));
            }
        }
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