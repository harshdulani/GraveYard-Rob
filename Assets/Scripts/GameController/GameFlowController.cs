using System;
using System.Collections;
using System.Net;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowController : MonoBehaviour
{
    private void OnGameplayBegin()
    {
        //fire an event that the player will listen to

        //objective changes to "find recently dug grave" or something
        //when user finds targetGrave, turn on targetCanvas and change objective title
        //Show ui splash text in center of screen saying
        //enemies/ghosts approaching or something doesn't feel right (and subsequent where did these ghosts come from?)
        //enemy wave controller starts its job
    }
}