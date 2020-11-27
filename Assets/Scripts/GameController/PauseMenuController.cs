using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum pauseMenuOptions
{
    Exit,
    Resume
};
public class PauseMenuController : MonoBehaviour
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
    
    public int SelectedMenuOption //there because does setter computation, also may be used by main menu nav
    {
        get => _selectedMenuOption;
        private set
        {
            if (value == -1)
                value = _totalOptionCount - 1;
            _selectedMenuOption = value % _totalOptionCount;
            _allowedToScroll = false;
            StartCoroutine(WaitForScrollingAgain());
        }
    }
    
    private int _selectedMenuOption = 0;

    private float _waitBeforeScrolling = 1f;
    private int _totalOptionCount = 2;
    private bool _allowedToScroll = true;

    private static readonly pauseMenuOptions _selection;
    
    public Animator cameraAnim;
    
    private static readonly int RightKeyPress = Animator.StringToHash("rightKeyPress");
    private static readonly int LeftKeyPress = Animator.StringToHash("leftKeyPress");
    
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
            MakeSelection((pauseMenuOptions) Mathf.Abs(SelectedMenuOption));
        }
    }

    private void MakeSelection(pauseMenuOptions option)
    {
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