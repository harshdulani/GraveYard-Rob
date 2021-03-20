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

    public GameObject blackOverlay, gameIntro;
    public Text pressToC;

    private bool _waitForUser, flashAlpha;

    private readonly Color _clearColor = new Color(1f, 1f, 1f, 0f);

    private WaitForSecondsRealtime pointOne = new WaitForSecondsRealtime(0.1f);

    private void Update()
    {
        if (!_waitForUser) return;

        if (!Input.GetKeyDown(KeyCode.Space)) return;

        HideGameIntro();
    }

    public void ShowGameIntro()
    {
        StartCoroutine(SlowDownTime());
        gameIntro.gameObject.SetActive(true);
        _waitForUser = true;
    }

    private void HideGameIntro()
    {
        StartCoroutine(RampUpTime());
        gameIntro.gameObject.SetActive(false);
        _waitForUser = false;
    }

    private IEnumerator SlowDownTime()
    {
        while (Time.timeScale >= 0.1f)
        {
            yield return pointOne;
            Time.timeScale -= 0.1f;
        }

        Time.timeScale = 0f;
    }

    private IEnumerator RampUpTime()
    {
        while (Time.timeScale <= 1f)
        {
            Time.timeScale += 0.1f;
            yield return pointOne;
        }

        Time.timeScale = 1f;
    }
}
