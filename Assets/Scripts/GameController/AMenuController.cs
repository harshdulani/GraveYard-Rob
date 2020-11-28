using System.Collections;
using UnityEngine;

public abstract class AMenuController : MonoBehaviour
{
    protected int SelectedMenuOption //there because does setter computation, also may be used by main menu nav
    {
        get => _selectedMenuOption;
        set
        {
            if (value == -1)
                value = _totalOptionCount.Value - 1;
            _selectedMenuOption = value % _totalOptionCount.Value;
            _allowedToScroll = false;
            StartCoroutine(WaitForScrollingAgain());
        }
    }

    private int _selectedMenuOption = 0;

    protected float _waitBeforeScrolling = 1f;
    protected int? _totalOptionCount = null;
    protected bool _allowedToScroll = true;

    public Animator cameraAnim;
    
    protected static readonly int RightKeyPress = Animator.StringToHash("rightKeyPress");
    protected static readonly int LeftKeyPress = Animator.StringToHash("leftKeyPress");

    protected abstract void MakeSelection(int selection);

    private IEnumerator WaitForScrollingAgain()
    {
        //because pause menus make timescale 0
        yield return new WaitForSecondsRealtime(_waitBeforeScrolling);
        _allowedToScroll = true;
    }
}
