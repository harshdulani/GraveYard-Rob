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

public class MainMenuController : MonoBehaviour
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

    private int _selectedMenuOption = 2;

    private float waitBeforeScrolling = 1f;
    private int _totalOptionCount = 4;
    private bool _initiated, _allowedToScroll = true;

    private static readonly mainMenuOptions _selection;
    
    public Animator cameraAnim;
    public GameObject optionTextMeshHolder;
    
    private static readonly int RightKeyPress = Animator.StringToHash("rightKeyPress");
    private static readonly int LeftKeyPress = Animator.StringToHash("leftKeyPress");
    
    private void Start()
    {
        _totalOptionCount = Enum.GetValues(typeof(mainMenuOptions)).Length;
        OnLoadMainMenu();
    }
    
    private void Update()
    {
        if (!_initiated) return;
        if(!_allowedToScroll) return;
        
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
            MakeSelection((mainMenuOptions) Mathf.Abs(SelectedMenuOption));
        }
    }

    private void MakeSelection(mainMenuOptions option)
    {
        print("selected " + option);
        switch (option)
        {
            case mainMenuOptions.Exit:
                Application.Quit();
                break;
            case mainMenuOptions.Play:
                GameFlowEvents.current.InvokeGameplayStart();
                break;
            case mainMenuOptions.About:
                print("lmao later");
                break;
            case mainMenuOptions.Settings:
                print("lmao later");
                break;
        }
    }

    private void OnLoadMainMenu()
    {
        var loading = SceneManager.LoadSceneAsync("GraveyardScene", LoadSceneMode.Additive);
        
        loading.completed += (asyncOperation) =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("GraveyardScene"));

            optionTextMeshHolder.SetActive(true);

            _initiated = true;
        };
    }

    private IEnumerator WaitForScrollingAgain()
    {
        var targetTime = Time.time + waitBeforeScrolling;
        while (Time.time <= targetTime)
        {
            yield return new WaitForSeconds(0.2f);
        }
        _allowedToScroll = true;
    }
}
