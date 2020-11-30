using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvasController : MonoBehaviour
{
    public Image healthBar;
    public Text healthText;

    private void OnEnable()
    {
        PlayerEvents.current.updateHealth += UpdateHealthBar;
        PlayerEvents.current.updateHealth += UpdateHealthText;
    }

    private void OnDisable()
    {
        PlayerEvents.current.updateHealth -= UpdateHealthBar;
        PlayerEvents.current.updateHealth -= UpdateHealthText;
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)(PlayerStats.main.playerHealth) / (float)(PlayerStats.main.maxHealth);
    }

    private void UpdateHealthText()
    {
        healthText.text = PlayerStats.main.playerHealth.ToString();
    }
}
