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
        _ = OnHealthChange(0, AttackType.Heal);
        _ = OnStaminaChange(0);
    }

    private bool OnHealthChange(int amount, AttackType type)
    {
        if (playerHealth - amount < 0)
        {
            playerHealth -= amount;
            return false;
        }
        
        if (type == AttackType.HeavyAttack)
            _playerController.OnPlayerTakeHit();
        else if (type == AttackType.LightAttack)
            _playerController.OnPlayerTakeBump();

        if(playerHealth <= 0.3f * maxHealth)
            PlayerCanvasController.main.HealthAlarm();
        
        playerHealth -= amount;
        playerHealth = Mathf.Clamp(playerHealth, 0, maxHealth);
        
        PlayerCanvasController.main.UpdateHealth();
        
        return true;
        //return true if there is enough health/player is still alive after the move
    }

    private bool OnStaminaChange(int amount)
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
}
