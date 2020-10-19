using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int maxHealth = 200;
    public int bumpDamage = 50;
    public int enemyHealth;

    private void Awake()
    {
        enemyHealth = maxHealth;
    }

    public void PlayerTakeHit(int damage)
    {
        enemyHealth -= damage;        
    }

    public void EnemyHeal(int healed)
    {
        enemyHealth += healed;
    }
}
