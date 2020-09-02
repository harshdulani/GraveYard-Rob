using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Image healthBar;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
    }

    public void DecreaseHealth(int amt)
    {        
        playerStats.playerHealth -= amt;
        UpdateHealthBar();
        if(playerStats.playerHealth <= 0)
        {
            //die
            print("YOU DIED.");
            Destroy(gameObject);
        }
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)(playerStats.playerHealth) / (float)(playerStats.maxHealth);
        print("health updated to " + (float)(playerStats.playerHealth));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Projectile"))
        {
            DecreaseHealth(collision.gameObject.GetComponent<ProjectileController>().projectileDamage);
            print("projectile hit w " + collision.gameObject.name);
            Destroy(collision.gameObject);
        }
        if(collision.gameObject.CompareTag("Enemy"))
        {
            print("enemy hit w " + collision.gameObject.name);
            DecreaseHealth(collision.gameObject.GetComponent<EnemyShooting>().bumpDamage);
        }
    }
}