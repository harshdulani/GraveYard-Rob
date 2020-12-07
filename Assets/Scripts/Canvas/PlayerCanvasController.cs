using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvasController : MonoBehaviour
{
    public static PlayerCanvasController main;
    
    public Image healthBar;
    public Text healthText;

    public Image staminaBar;
    public Text staminaText;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(this);
    }

    public void UpdateHealth()
    {
        healthBar.fillAmount = (float)(PlayerStats.main.playerHealth) / (float)(PlayerStats.main.maxHealth);
        healthText.text = PlayerStats.main.playerHealth.ToString();
    }
    
    public void UpdateStamina()
    {
        staminaBar.fillAmount = (float)(PlayerStats.main.playerStamina) / (float)(PlayerStats.main.maxStamina);
        staminaText.text = PlayerStats.main.playerStamina.ToString();
    }
}
