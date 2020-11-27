using System;
using System.Collections;
using UnityEngine;

public abstract class AMenuController : MonoBehaviour
{
    protected int SelectedMenuOption //there because does setter computation, also may be used by main menu nav
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

    protected float _waitBeforeScrolling = 1f;
    protected int _totalOptionCount;
    private bool _allowedToScroll = true;

    public Animator cameraAnim;
    
    protected static readonly int RightKeyPress = Animator.StringToHash("rightKeyPress");
    protected static readonly int LeftKeyPress = Animator.StringToHash("leftKeyPress");

    protected abstract void MakeSelection(Enum selection);

    protected IEnumerator WaitForScrollingAgain()
    {
        var targetTime = Time.time + _waitBeforeScrolling;
        while (Time.time <= targetTime)
        {
            yield return new WaitForSeconds(0.2f);
        }
        _allowedToScroll = true;
    }
}
