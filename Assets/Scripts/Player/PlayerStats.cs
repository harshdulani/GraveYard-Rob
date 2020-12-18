using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats main;
    
    public int playerHealth;
    public int maxHealth = 1000;

    public int playerStamina;
    public int maxStamina = 750;

    public int lightAttackDamage = 100;
    public int heavyAttackDamage = 250;

    public int jumpStaminaCost = 150;

    public int lightAttackStaminaCost = 50;
    public int heavyAttackStaminaCost = 150;

    private PlayerController _playerController;
    private int _sampleEnemyBumpDamage, _sampleEnemyMeleeDamage;
    
    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        PlayerEvents.current.healthChange += OnHealthChange;
        PlayerEvents.current.staminaChange += OnStaminaChange;
    }

    private void OnDisable()
    {
        PlayerEvents.current.healthChange -= OnHealthChange;
        PlayerEvents.current.staminaChange -= OnStaminaChange;
    }

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        
        playerHealth = maxHealth;
        playerStamina = maxStamina;
        _ = OnHealthChange(0);
        _ = OnStaminaChange(0);
    }

    public bool OnHealthChange(float amount)
    {
        if (playerHealth - Mathf.CeilToInt(amount) < 0) return false;

        if (_sampleEnemyBumpDamage == default)
            if(SetDemoEnemyValues())
            {
                if (amount == _sampleEnemyMeleeDamage)
                    _playerController.OnPlayerTakeHit();
                else
                    _playerController.OnPlayerTakeBump();
            }
            
        
        if(playerHealth <= 0.3f * maxHealth)
            PlayerCanvasController.main.HealthAlarm();
        
        playerHealth -= Mathf.CeilToInt(amount);
        playerHealth = Mathf.Clamp(playerHealth, 0, maxHealth);
        
        PlayerCanvasController.main.UpdateHealth();
        
        return true;
    
        //return true if there is enough health/player is still alive after the move
    }

    public bool OnStaminaChange(float amount)
    {
        if (playerStamina - Mathf.CeilToInt(amount) < 0)
        {
            PlayerCanvasController.main.StaminaAlarm();
            return false;
        }
        
        playerStamina -= Mathf.CeilToInt(amount);
        playerStamina = Mathf.Clamp(playerStamina, 0, maxStamina);

        PlayerCanvasController.main.UpdateStamina();
        
        return true;
        
        //return true if there is enough stamina for requested move
    }

    private bool SetDemoEnemyValues()
    {
        EnemyStats x;
        
        //try to find an EnemyStats and assign it to x, and then test that value
        if ((x = FindObjectOfType<EnemyStats>()))
        {
            _sampleEnemyBumpDamage = x.bumpDamage;
            _sampleEnemyMeleeDamage = x.meleeDamage;
            return true;
        }
        else
            return false;
    }
}
