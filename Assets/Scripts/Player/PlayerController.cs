using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Image healthBar;
    public Text healthText;
    
    public event Action PlayerDeath, PlayerBirth;
    
    private PlayerStats _playerStats;

    private void Start()
    {
        PlayerBirth?.Invoke();
        
        _playerStats = GetComponent<PlayerStats>();
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
            PlayerDeath?.Invoke();    //this ? checks and only invokes if this Action is not null
            Destroy(gameObject, 0.5f);
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