using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum pauseMenuOptions
{
    Exit,
    Resume
};
public class PauseMenuController : AMenuController
{
    #region Singleton, Awake() lies inside

    private static PauseMenuController main;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);
    }

    #endregion

    public bool _isPaused;
    private static readonly pauseMenuOptions _selection;

    private Transform _player, _mainCam;

    private void Start()
    {
        AudioController = GetComponent<MenuAudioController>();
        _totalOptionCount = Enum.GetValues(typeof(pauseMenuOptions)).Length;
        SelectedMenuOption = 1;
        _mainCam = Camera.main.transform;
        OnLoadPauseMenu();
    }

    private void Update()
    {
        if (!_allowedToScroll) return;

        if (_isPaused)
        {
            if (Input.GetAxisRaw("Horizontal") == 1f)
            {
                cameraAnim.SetTrigger(RightKeyPress);
                SelectedMenuOption++;
            }
            else if (Input.GetAxisRaw("Horizontal") == -1f)
            {
                cameraAnim.SetTrigger(LeftKeyPress);
                SelectedMenuOption--;
            }
            if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Fire1"))
            {
                MakeSelection(SelectedMenuOption);
            }
            else if (Input.GetButtonDown("Cancel"))
            {
                GameFlowEvents.current.InvokeGameplayResume();
                OnGameplayResume();
            }
        }
        else
        {
            if (!GameStats.current.isGamePlaying) return;
            if (!GameStats.current.isPlayerAlive) return;
            
            if (Input.GetButtonDown("Cancel"))
            {
                GameFlowEvents.current.InvokeGameplayPause();
                OnGameplayPause();
            }
        }
    }

    protected override void MakeSelection(int selection)
    {
        var option = (pauseMenuOptions) Mathf.Abs(selection);
        print("pause selected " + option);
        switch (option)
        {
            case pauseMenuOptions.Exit:
                ExitGame();
                break;
            case pauseMenuOptions.Resume:
                GameFlowEvents.current.InvokeGameplayResume();
                OnGameplayResume();
                break;
            default:
                print("not processed properly");
                break;
        }
    }

    private void OnLoadPauseMenu()
    {
        _player = PlayerStats.main.transform.GetChild(8).transform;
    }

    private void OnGameplayPause()
    { 
        if(!GameStats.current.isPlayerAlive) return;
        
        GameStats.current.isGamePlaying = false;
        _isPaused = true;
        Time.timeScale = 0f;

        foreach (var rootGameObject in SceneManager.GetSceneByName("PauseMenuScene").GetRootGameObjects())
        {
            //these were set inactive by the MainMenuController when spawning them
            rootGameObject.SetActive(true);
        }
    }
    
    private void OnGameplayResume()
    {
        GameStats.current.isGamePlaying = true;
        _isPaused = false;
        Time.timeScale = 1f;
        
        foreach (var rootGameObject in SceneManager.GetSceneByName("PauseMenuScene").GetRootGameObjects())
        {
            if(!rootGameObject.name.Equals("PauseMenuController"))
                rootGameObject.SetActive(false);
        }
    }

    private void ExitGame()
    {
        //quit the damn game for now.
        //we'll think of restarting by only loading a partial graveyard duplicate later
        Application.Quit();
    }
}