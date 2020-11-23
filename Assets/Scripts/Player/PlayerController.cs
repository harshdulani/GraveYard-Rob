﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Image healthBar;
    public Text healthText;

    private PlayerStats _playerStats;
    private static readonly int PlayerBorn = Animator.StringToHash("playerBorn");
    private static readonly int CycleWeapon = Animator.StringToHash("cycleWeapon");

    private void Start()
    {
        PlayerEvents.current.InvokePlayerBirth();
        
        _playerStats = GetComponent<PlayerStats>();
        UpdateHealthBar();
        UpdateHealthText();
    }

    private void OnEnable()
    {
        GameFlowEvents.current.gameplayStart += OnGameplayStart;
    }

    private void OnDisable()
    {
        GameFlowEvents.current.gameplayStart -= OnGameplayStart;
    }

    private void OnGameplayStart()
    {
        GetComponent<CharacterController>().enabled = false;
        StartCoroutine(StartAnimation());
        GetComponentInChildren<PlayerWeaponController>().gameObject.SetActive(false);
    }

    private IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<Animator>().SetBool(PlayerBorn, true);
    }

    public void OnClimbDownFence()
    {
        GetComponent<Animator>().SetTrigger(CycleWeapon);
        GetComponent<CharacterController>().enabled = true;
        GetComponentInChildren<PlayerWeaponController>().gameObject.SetActive(true);
        //slide in objective canvas & player canvas
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