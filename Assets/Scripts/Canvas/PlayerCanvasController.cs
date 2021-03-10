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

    private Canvas _canvas;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(this);
    }

    private void OnEnable()
    {
        GameFlowEvents.current.gameOver += OnGameOver;
        GameFlowEvents.current.gameplayPause += OnPause;
        GameFlowEvents.current.gameplayResume += OnResume;
    }

    private void OnDisable()
    {
        GameFlowEvents.current.gameOver -= OnGameOver;
        GameFlowEvents.current.gameplayPause -= OnPause;
        GameFlowEvents.current.gameplayResume -= OnResume;
    }

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        
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
    
    private void OnGameOver()
    {
        gameObject.SetActive(false);
    }

    private void OnPause()
    {
        _canvas.enabled = false;
    }

    private void OnResume()
    {
        _canvas.enabled = true;
    }
}
