using System.Collections;
using System.Collections.Generic;
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

    private bool _isPaused;
    
    private static readonly pauseMenuOptions _selection;

    private Transform _player;

    private void Start()
    {
        _totalOptionCount = System.Enum.GetValues(typeof(mainMenuOptions)).Length;
        
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
            if (Input.GetButtonDown("Submit"))
            {
                MakeSelection(SelectedMenuOption);
            }
        }
        else
        {
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
        print("selected " + option);
        switch (option)
        {
            case pauseMenuOptions.Exit:
                ExitGame();
                break;
            case pauseMenuOptions.Resume:
                OnResume();
                break;
            default:
                print("not processed properly");
                break;
        }
    }

    private void OnLoadPauseMenu()
    {
        //assign player reference
        _player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(8).transform;
        print("Found player " + _player.name);
    }

    private void OnGameplayPause()
    {
        _isPaused = true;
        foreach (var rootGameObject in SceneManager.GetSceneByName("PauseMenuScene").GetRootGameObjects())
        {
            rootGameObject.SetActive(true);
            print(true);
        }

        Time.timeScale = 0f;

        cameraAnim.transform.GetChild(1).GetComponent<CinemachineFreeLook>().m_Follow = _player;
        cameraAnim.transform.GetChild(1).GetComponent<CinemachineFreeLook>().m_LookAt = _player;
        //camera that looks at player for resume
        //camera that looks at exit gate for exit
    }
    
    private void OnResume()
    {
        foreach (var rootGameObject in SceneManager.GetSceneByName("PauseMenuScene").GetRootGameObjects())
        {
            rootGameObject.SetActive(false);
        }
    }

    private void ExitGame()
    {
        //quit the damn game for now.
        Application.Quit();
    }
}