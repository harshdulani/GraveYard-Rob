using System;
using System.Collections;
using System.Net;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void OnGameplayBegin()
    {
        //move all these functions to a MainMenuController - X
        //all other UI canvases will be turned off - X
        
        //fire an event that the player will listen to
        //objectives canvas & player canvas slides in

        //objective changes to "find recently dug grave" or something
        //when user finds targetGrave, turn on targetCanvas and change objective title
        //Show ui splash text in center of screen saying
        //enemies/ghosts approaching or something doesn't feel right (and subsequent where did these ghosts come from?)
        //enemy wave controller starts its job
    }
}