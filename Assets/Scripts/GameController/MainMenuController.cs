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

    private static MainMenuController main;

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

    private bool _initiated, _inGame;

    private static readonly mainMenuOptions _selection;
    
    public GameObject optionTextMeshHolder;
    
    private void Start()
    {
        _totalOptionCount = Enum.GetValues(typeof(mainMenuOptions)).Length;
        
        SelectedMenuOption = 2;
        
        if (SceneManager.GetSceneByName("GraveyardScene").isLoaded) return;
        OnLoadMainMenu();
    }
    
    private void Update()
    {
        if (!_initiated) return;
        if (_inGame) return;
        if (!_allowedToScroll) return;
        
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

        if (Input.GetButtonDown("Submit"))
        {
            MakeSelection(SelectedMenuOption);
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
                print("lmao later");
                //use this space to unset _inGame when coming back from this section
                break;
            case mainMenuOptions.Settings:
                print("lmao later");
                //use this space to unset _inGame when coming back from this section
                break;
            default:
                print("not processed properly");
                break;
        }

        _inGame = true;
    }

    private void OnGameplayStart()
    {
        SceneManager.UnloadSceneAsync("MainMenuScene");
    }

    private void OnLoadMainMenu()
    {
        var loading = SceneManager.LoadSceneAsync("GraveyardScene", LoadSceneMode.Additive);

        loading.completed += OnSceneLoadingComplete;
    }

    private void OnSceneLoadingComplete(AsyncOperation operation)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("GraveyardScene"));

        optionTextMeshHolder.SetActive(true);

        _initiated = true;
        operation.completed -= OnSceneLoadingComplete;
    }
}
