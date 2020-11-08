using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyStats : MonoBehaviour
{
    //TODO enum for enemy type
    public int maxHealth = 200;
    public int bumpDamage = 50;
    public int meleeDamage = 75;
    public float waitBeforeAttackTime = 0.5f;
    
    public int enemyHealth;

    private void Awake()
    {
        enemyHealth = maxHealth;
    }

    public bool TakeHit(int damage)
    {
        enemyHealth -= damage;
        
        //returns true if dead
        return enemyHealth <= 0;
    }

    public void GetHealed(int healed)
    {
        enemyHealth += healed;
    }
}
