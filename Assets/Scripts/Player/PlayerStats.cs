using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats main;
    
    public int playerHealth;
    public int maxHealth = 1000;

    public int playerStamina;
    public int maxStamina = 750;

    public const int LightAttackDamage = 100;
    public const int HeavyAttackDamage = 250;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        playerHealth = maxHealth;
    }

    public bool TakeHit(int damage)
    {
        playerHealth -= damage;
        return playerHealth <= 0;
    }

    public void GetHealed(int healed)
    {
        playerHealth += healed;
    }

    public void UseStamina(int amount)
    {
        playerStamina -= amount;
    }

    public void ReplenishStamina(int amount)
    {
        playerStamina += amount;
    }
}
