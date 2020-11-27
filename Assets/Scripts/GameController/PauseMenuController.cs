using System.Collections;
using System.Collections.Generic;
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
   
    private static readonly pauseMenuOptions _selection;

    private void Start()
    {
        _totalOptionCount = System.Enum.GetValues(typeof(mainMenuOptions)).Length;
        
        OnLoadPauseMenu();
    }

    private void Update()
    {
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
        var option = (pauseMenuOptions) Mathf.Abs(selection);
        print("selected " + option);
        switch (option)
        {
            case pauseMenuOptions.Exit:
                RestartGame();
                break;
            case pauseMenuOptions.Resume:
                GameFlowEvents.current.InvokeGameplayPause();
                OnGameplayPause();
                break;
            default:
                print("not processed properly");
                break;
        }
    }

    private void OnLoadPauseMenu()
    {
        //assign player reference
    }

    private void OnGameplayPause()
    {
        //slow down time to hyper low amount
        //camera that looks at player for resume
        //camera that looks at exit gate for exit
    }

    private void RestartGame()
    {
        //unload pausemenuscene
        //timescale back to 1.0f
        //tps camera turned on
    }
    
    private IEnumerator WaitForScrollingAgain()
    {
        var targetTime = Time.time + _waitBeforeScrolling;
        while (Time.time <= targetTime)
        {
            yield return new WaitForSeconds(0.2f);
        }
        _allowedToScroll = true;
    }
}