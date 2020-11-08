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
        PlayerEvents.current.InvokePlayerBirth();
        
        _playerStats = GetComponent<PlayerStats>();
        UpdateHealthBar();
        UpdateHealthText();
    }

    public void DecreaseHealth(int amt)
    {
        if (_playerStats.TakeHit(amt))
        {
            //die
            print("YOU DIED.");
            PlayerEvents.current.InvokePlayerDeath();
            Destroy(gameObject);
        }

        UpdateHealthBar();
        UpdateHealthText();
    }

    private void UpdateHealthBar()
    {
        //this needs to move out
        healthBar.fillAmount = (float)(_playerStats.playerHealth) / (float)(_playerStats.maxHealth);
    }

    private void UpdateHealthText()
    {
        //this needs to move out
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