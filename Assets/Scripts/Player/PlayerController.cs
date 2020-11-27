using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Image healthBar;
    public Text healthText;

    private PlayerStats _playerStats;
    private PlayerWeaponController _weaponController;

    private static readonly int PlayerBorn = Animator.StringToHash("playerBorn");

    private void Start()
    {
        _weaponController = GetComponentInChildren<PlayerWeaponController>();
        _weaponController.gameObject.SetActive(false);
        PlayerEvents.current.InvokePlayerBirth();
        
        _playerStats = GetComponent<PlayerStats>();
        UpdateHealthBar();
        UpdateHealthText();

        //so that hes not visible on main menu
        transform.localScale = new Vector3(0, 0, transform.localScale.z);
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
        transform.localScale = Vector3.one;
        GetComponent<Animator>().SetBool(PlayerBorn, true);
    }

    public void OnClimbDownFence()
    {
        GetComponent<PlayerCombat>().SwapWeapon();
        //so that attacks can happen
        GameStats.current.isGamePlaying = true;
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