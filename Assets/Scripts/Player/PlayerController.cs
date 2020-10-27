using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Image healthBar;
    public Text healthText;
    
    private PlayerStats _playerStats;

    private void Start()
    {
        _playerStats = FindObjectOfType<PlayerStats>();
        UpdateHealthBar();
        UpdateHealthText();
    }

    public void DecreaseHealth(int amt)
    {        
        _playerStats.playerHealth -= amt;
        UpdateHealthBar();
        UpdateHealthText();
        if (_playerStats.playerHealth <= 0)
        {
            //die
            print("YOU DIED.");
            Destroy(gameObject);
        }
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)(_playerStats.playerHealth) / (float)(_playerStats.maxHealth);
    }

    private void UpdateHealthText()
    {
        healthText.text = _playerStats.playerHealth.ToString();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.CompareTag("Projectile"))
        {
            DecreaseHealth(hit.gameObject.GetComponent<ProjectileController>().projectileDamage);
            print("projectile hit w " + hit.gameObject.name);
            Destroy(hit.gameObject);
        }
    }
}