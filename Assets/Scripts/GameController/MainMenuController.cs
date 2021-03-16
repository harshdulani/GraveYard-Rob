using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum mainMenuOptions
{
    Exit,
    Settings,
    Play,
    About
};

public class MainMenuController : AMenuController
{
    #region Singleton, Awake() lies inside

    private static MainMenuController _main;

    private void Awake()
    {
        if (_main == null)
        {
            _main = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);
    }

    #endregion

    private bool _initiated, _insideMenu, _isInsideSettingsNotAbout;//false when inside about and not settings

    private static readonly mainMenuOptions _selection;
    
    public GameObject optionTextMeshHolder;
    public GameObject aboutCanvas, settingsCanvas, helpCanvas;

    private void Start()
    {
        AudioController = GetComponent<MenuAudioController>();
        
        //this is set to 0.5f in GameOver Screen
        Time.timeScale = 1f;

        _totalOptionCount = Enum.GetValues(typeof(mainMenuOptions)).Length;
        
        SelectedMenuOption = 2;
        
        if (SceneManager.GetSceneByName("GraveyardScene").isLoaded) return;
        OnLoadMainMenu();
    }
    
    private void Update()
    {
        if (!_initiated) return;
        if (!_allowedToScroll) return;
        if (_insideMenu)
        {
            if (!Input.GetButtonDown("Cancel")) return;
            
            if(_isInsideSettingsNotAbout)
                ToggleSettingsContent(false);
            else
                ToggleAboutContent(false);
        }
        else
        {
            if (Input.GetAxisRaw("Horizontal") == 1f)
            {
                cameraAnim.SetTrigger(RightKeyPress);
                SelectedMenuOption++;
                
                TurnOffHelpCanvas();
            }
            else if (Input.GetAxisRaw("Horizontal") == -1f)
            {
                cameraAnim.SetTrigger(LeftKeyPress);
                SelectedMenuOption--;
                
                TurnOffHelpCanvas();
            }

            if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Fire1"))
            {
                MakeSelection(SelectedMenuOption);
                
                TurnOffHelpCanvas();
            }
        }
    }

    protected override void MakeSelection(int selection)
    {
        var option = (mainMenuOptions) Mathf.Abs(selection);
        print("selected " + option);
        switch (option)
        {
            case mainMenuOptions.Exit:
                Application.Quit();
                break;
            case mainMenuOptions.Play:
                GameFlowEvents.current.InvokeGameplayStart();
                OnGameplayStart();
                break;
            case mainMenuOptions.About:
                ToggleAboutContent(true);
                break;
            case mainMenuOptions.Settings:
                ToggleSettingsContent(true);
                break;
            default:
                print("not processed properly");
                break;
        }
    }

    private void OnGameplayStart()
    {
        Destroy(gameObject);
        SceneManager.UnloadSceneAsync("MainMenuScene");
    }

    private void OnLoadMainMenu()
    {
        var loadingLevel = SceneManager.LoadSceneAsync("GraveyardScene", LoadSceneMode.Additive);
        var loadingPauseMenu = SceneManager.LoadSceneAsync("PauseMenuScene", LoadSceneMode.Additive);
        var loadingGameOver = SceneManager.LoadSceneAsync("GameOverScene", LoadSceneMode.Additive);

        loadingLevel.completed += OnLevelLoadingComplete;
        StartCoroutine(LoadSceneCoroutine(loadingPauseMenu, "PauseMenuScene"));
        StartCoroutine(LoadSceneCoroutine(loadingGameOver, "GameOverScene"));
    }

    private void OnLevelLoadingComplete(AsyncOperation operation)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("GraveyardScene"));

        optionTextMeshHolder.SetActive(true);

        _initiated = true;
        operation.completed -= OnLevelLoadingComplete;
    }

    private IEnumerator LoadSceneCoroutine(AsyncOperation operation, string sceneName)
    {
        while (!operation.isDone)
        {
            yield return null;
        }

        foreach (var obj in SceneManager.GetSceneByName(sceneName).GetRootGameObjects())
        {
            obj.SetActive(false);
        }
        //a notifier to "intermediary" scene/black image at the start of the game that waits for everything to load
    }

    private void ToggleAboutContent(bool showAbout)
    {
        aboutCanvas.SetActive(showAbout);
        _insideMenu = showAbout;
        _isInsideSettingsNotAbout = false;
    }

    private void ToggleSettingsContent(bool showSettings)
    {
        settingsCanvas.SetActive(showSettings);
        _insideMenu = showSettings;
        _isInsideSettingsNotAbout = true;
    }

    private void TurnOffHelpCanvas()
    {
        if(helpCanvas)
            Destroy(helpCanvas);
    }
}
