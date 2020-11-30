using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerHealth;
    public int maxHealth = 1000;

    public const int lightAttackDamage = 100;
    public const int HeavyAttackDamage = 250;

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
}
