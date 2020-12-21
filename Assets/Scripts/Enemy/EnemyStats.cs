using UnityEngine;

public enum EnemyType
{
    Ghost,
    Demon
}

public class EnemyStats : MonoBehaviour
{
    public EnemyType type;
    
    public int maxHealth = 200;
    public int bumpDamage = 50;
    public int attackDamage = 75;
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
