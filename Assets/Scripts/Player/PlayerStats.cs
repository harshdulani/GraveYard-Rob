using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerHealth;
    public int maxHealth = 1000;

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
