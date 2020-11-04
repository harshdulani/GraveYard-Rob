using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerHealth;
    public int maxHealth = 1000;

    public bool isAiming = false;

    private void Start()
    {
        playerHealth = maxHealth;
    }

    public void PlayerTakeHit(int damage)
    {
        playerHealth -= damage;
        if (IsPlayerDead())
            print("Game Over");
        //reflect in UI
        //consider passing message about player health changed
    }

    public void PlayerHeal(int healed)
    {
        playerHealth += healed;
    }

    private bool IsPlayerDead()
    {
        if (playerHealth <= 0)
            return true;
        else
            return false;
    }
}
