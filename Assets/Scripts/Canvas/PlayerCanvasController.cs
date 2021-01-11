using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvasController : MonoBehaviour
{
    public static PlayerCanvasController main;
    
    public Image healthBar;
    public Text healthText;
    private FlashRepeatedly _healthFlasher;

    public Image staminaBar;
    public Text staminaText;
    private FlashRepeatedly _staminaFlasher;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        _healthFlasher = healthBar.transform.parent.GetChild(4).GetComponent<FlashRepeatedly>();
        _staminaFlasher = staminaBar.transform.parent.GetChild(4).GetComponent<FlashRepeatedly>();
    }

    public void UpdateHealth()
    {
        healthBar.fillAmount = (float)(PlayerStats.main.playerHealth) / (float)(PlayerStats.main.maxHealth);
        healthText.text = PlayerStats.main.playerHealth + " / " + PlayerStats.main.maxHealth;;
    }
    
    public void UpdateStamina()
    {
        staminaBar.fillAmount = (float)(PlayerStats.main.playerStamina) / (float)(PlayerStats.main.maxStamina);
        staminaText.text = PlayerStats.main.playerStamina + " / " + PlayerStats.main.maxStamina;
    }

    public void HealthAlarm()
    {
        _healthFlasher.StartFlashing();
    }

    public void StaminaAlarm()
    {
        _staminaFlasher.StartFlashing();
    }
}
