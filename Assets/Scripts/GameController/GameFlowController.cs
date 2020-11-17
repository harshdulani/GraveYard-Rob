using System;
using System.Collections;
using System.Net;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum mainMenuOptions
{
    Exit,
    Settings,
    Play,
    About
};
public class GameFlowController : MonoBehaviour
{
    #region Singleton, Awake() lies inside

    private static GameFlowController main;

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

    public int _selectedMenuOption = 2;

    private float waitBeforeScrolling = 1f;
    private int _totalOptionCount = 4;
    private bool _initiated, _allowedToScroll = true;

    private static readonly mainMenuOptions _selection;
    
    public Animator cameraAnim;
    public GameObject optionTextMeshHolder;
    
    private static readonly int RightKeyPress = Animator.StringToHash("rightKeyPress");
    private static readonly int LeftKeyPress = Animator.StringToHash("leftKeyPress");

    //this object exists in the MainMenuScene
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
        
        if(Input.GetButtonDown("Submit"))
            print("selected " + (mainMenuOptions)Mathf.Abs(SelectedMenuOption));
    }

    private void OnLoadMainMenu()
    {
        //not loading MainMenu first and then graveyard because then MainMenu becomes active scene and lightmaps are not loaded properly
        var loading = SceneManager.LoadSceneAsync("MainMenuScene", LoadSceneMode.Additive);
        
        loading.completed += (asyncOperation) =>
        {
            cameraAnim = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject
                .GetComponent<Animator>();
            
            optionTextMeshHolder = cameraAnim.transform.parent.GetChild(1).gameObject;
            
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