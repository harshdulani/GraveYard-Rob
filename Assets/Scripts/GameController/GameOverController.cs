using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameOverOptions
{
    Retry,
    Quit
}

public class GameOverController : AMenuController
{
    #region Singleton, Awake() lies inside

    private static GameOverController main;

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

    public Transform _lookAt;

    private bool _isGameOver;
    
    private static readonly GameOverOptions _selection;

    private void Start()
    {
        _totalOptionCount = Enum.GetValues(typeof(GameOverOptions)).Length;
        SelectedMenuOption = 0;
    }

    private void OnEnable()
    {
        GameFlowEvents.current.gameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameFlowEvents.current.gameOver -= OnGameOver;
    }

    private void Update()
    {
        if (!_allowedToScroll) return;

        if (!_isGameOver) return;
        
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

    private void OnGameOver()
    {
        foreach (var rootGameObject in SceneManager.GetSceneByName("GameOverScene").GetRootGameObjects())
        {
            //these were set inactive by the MainMenuController when spawning them
            rootGameObject.SetActive(true);
        }
        _isGameOver = true;
    }

    protected override void MakeSelection(int selection)
    {
        var option = (GameOverOptions) Mathf.Abs(selection);
        print("pause selected " + option);
        switch (option)
        {
            case GameOverOptions.Quit:
                ExitGame();
                break;
            case GameOverOptions.Retry:
                RetryGame();
                break;
            default:
                print("not processed properly");
                break;
        }
    }

    private void RetryGame()
    {
        foreach (var controller in GameObject.FindGameObjectsWithTag("GameController"))
        {
            Destroy(controller);
        }
        SceneManager.LoadScene("MainMenuScene");
    }
    
    private void ExitGame()
    {
        //quit the damn game for now.
        //we'll think of restarting by only loading a partial graveyard duplicate later
        Application.Quit();
    }
}