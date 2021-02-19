using System.Collections;
using UnityEngine;

public abstract class AMenuController : MonoBehaviour
{
    protected int SelectedMenuOption //there because does setter computation, also may be used by main menu nav
    {
        get => _selectedMenuOption;
        set
        {
            var oldValue = _selectedMenuOption;
            if (value == -1) //if you underflow option list, come back to first option
            {
                value = _totalOptionCount.Value - 1;
                AudioController.Scroll(true);
            }
            else if (value > oldValue)
                AudioController.Scroll(false);
            else if (value < oldValue)
                AudioController.Scroll(true);
            
            _selectedMenuOption = value % _totalOptionCount.Value;
            _allowedToScroll = false;
            StartCoroutine(WaitForScrollingAgain());
        }
    }

    private int _selectedMenuOption = 0;
    private float _waitBeforeScrolling = 1f;
    
    protected int? _totalOptionCount = null;
    protected bool _allowedToScroll = true;

    public Animator cameraAnim;
    protected MenuAudioController AudioController;
    
    protected static readonly int RightKeyPress = Animator.StringToHash("rightKeyPress");
    protected static readonly int LeftKeyPress = Animator.StringToHash("leftKeyPress");

    private WaitForSecondsRealtime _waitScrolling;
    
    protected abstract void MakeSelection(int selection);

    private IEnumerator WaitForScrollingAgain()
    {
        if(_waitScrolling == null)
            _waitScrolling = new WaitForSecondsRealtime(_waitBeforeScrolling);
        
        //because pause menus make timescale 0
        yield return _waitScrolling;
        _allowedToScroll = true;
    }
}
