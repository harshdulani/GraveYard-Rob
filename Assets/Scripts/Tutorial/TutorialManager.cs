using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    #region Singleton

    public static TutorialManager current;

    private void Awake()
    {
        if (current == null)
            current = this;
        else
            Destroy(current);
    }

    #endregion

    public GameObject gameIntro, playerCanvas;
    public SlideIntoScreen playerCanvasSlide;
    
    public Animation lmb, rmb, space;
    
    private bool _waitForUser, _isTutoring, _lmbDone, _rmbDone, _spaceDone;

    private readonly WaitForSecondsRealtime _pointOne = new WaitForSecondsRealtime(0.1f);
    private WaitForSeconds _pointFive = new WaitForSeconds(.5f);

    private void Update()
    {
        if (_waitForUser)
        {
            if (Input.GetButtonDown("Jump"))
                HideGameIntro();
        }
        else if (_isTutoring)
        {
            if (!_lmbDone && Input.GetButtonDown("Fire1"))
            {
                _lmbDone = true;
                lmb.Play();
            }
            else if (!_rmbDone && Input.GetButtonDown("Fire2"))
            {
                _rmbDone = true;
                rmb.Play();
            }
            else if (!_spaceDone && Input.GetButtonDown("Jump"))
            {
                _spaceDone = true;
                space.Play();
            }

            if (_lmbDone && _rmbDone && _spaceDone)
            {
                _isTutoring = false;
                lmb.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void ShowGameIntro()
    {
        StartCoroutine(SlowDownTime());
        gameIntro.gameObject.SetActive(true);
        _waitForUser = true;
    }

    private void HideGameIntro()
    {
        StopAllCoroutines();
        StartCoroutine(RampUpTime());
        gameIntro.gameObject.SetActive(false);
        _waitForUser = false;
    }

    public void ShowGlyphs()
    {
        StartCoroutine(StartTutorial());
    }

    private IEnumerator StartTutorial()
    {
        yield return _pointFive;
        
        playerCanvas.SetActive(true);
        playerCanvasSlide.StartSliding();
        
        _isTutoring = true;
        lmb.gameObject.SetActive(true);
        rmb.gameObject.SetActive(true);
        space.gameObject.SetActive(true);
    }
    
    private IEnumerator SlowDownTime()
    {
        while (Time.timeScale >= 0.1f)
        {
            yield return _pointOne;
            Time.timeScale -= 0.1f;
        }

        Time.timeScale = 0f;
    }

    private IEnumerator RampUpTime()
    {
        while (Time.timeScale <= 1f)
        {
            Time.timeScale += 0.1f;
            yield return _pointOne;
        }

        Time.timeScale = 1f;
    }
}
