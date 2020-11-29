using System;
using System.Collections;
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

    public Transform resumeText;

    public bool _isPaused;
    private static readonly pauseMenuOptions _selection;

    private Transform _player, _mainCam;

    private void Start()
    {
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
            if(SelectedMenuOption == 1)
                resumeText.transform.rotation = Quaternion.LookRotation(-_mainCam.transform.forward);
            
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
            else if (Input.GetButtonDown("Cancel"))
            {
                GameFlowEvents.current.InvokeGameplayResume();
                OnGameplayResume();
            }
        }
        else
        {
            if (!GameStats.current.isGamePlaying) return;
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
        _player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(8).transform;
        print("Found player " + _player.name);
    }

    private void OnGameplayPause()
    {
        GameStats.current.isGamePlaying = false;
        _isPaused = true;
        Time.timeScale = 0f;

        foreach (var rootGameObject in SceneManager.GetSceneByName("PauseMenuScene").GetRootGameObjects())
        {
            rootGameObject.SetActive(true);
        }
        
        cameraAnim.transform.GetChild(1).GetComponent<CinemachineFreeLook>().m_Follow = _player;
        cameraAnim.transform.GetChild(1).GetComponent<CinemachineFreeLook>().m_LookAt = _player;

        resumeText.gameObject.SetActive(true);
        resumeText.parent = _player;
        resumeText.localPosition = Vector3.zero + Vector3.back * 4.25f;
    }
    
    private void OnGameplayResume()
    {
        GameStats.current.isGamePlaying = true;
        _isPaused = false;
        Time.timeScale = 1f;

        resumeText.gameObject.SetActive(false);
        
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