using System;
using UnityEngine;

public enum SlideInDirection
{
    FromLeft,
    FromRight,
    FromTop,
    FromBottom
}

public class SlideIntoScreen : MonoBehaviour
{
    public SlideInDirection mySlideInDirection;

    private Vector3 _initialLocalPosition, _newLocalPosition;
    private float _time, _endTime;
    private bool _isSlidingIn;

    private RectTransform _rectTransform;
    [SerializeField] private AnimationCurve slideOverTimeCurve;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _initialLocalPosition = _rectTransform.localPosition;
        _endTime = slideOverTimeCurve.keys[slideOverTimeCurve.keys.Length - 1].time;

        switch (mySlideInDirection)
        {
            case SlideInDirection.FromLeft:
            case SlideInDirection.FromRight:
                _rectTransform.localPosition = _newLocalPosition = new Vector3(_initialLocalPosition.x * 2, _initialLocalPosition.y, _initialLocalPosition.z);
                break;
            case SlideInDirection.FromTop:
            case SlideInDirection.FromBottom:
                _rectTransform.localPosition = _newLocalPosition = new Vector3(_initialLocalPosition.x, _initialLocalPosition.y * 2, _initialLocalPosition.z);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Update()
    {
        if(_isSlidingIn)
            SlideIn();
    }

    private void SlideIn()
    {
        _time += Time.deltaTime;

        switch (mySlideInDirection)
        {
            case SlideInDirection.FromLeft:
            case SlideInDirection.FromRight:
                _rectTransform.localPosition = new Vector3(_newLocalPosition.x - _initialLocalPosition.x * slideOverTimeCurve.Evaluate(_time), _newLocalPosition.y, _newLocalPosition.z);
                break;
            case SlideInDirection.FromTop:
            case SlideInDirection.FromBottom:
                _rectTransform.localPosition = new Vector3(_newLocalPosition.x, _newLocalPosition.y - _initialLocalPosition.y * slideOverTimeCurve.Evaluate(_time), _newLocalPosition.z);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (_time >= _endTime)
            _isSlidingIn = false;
    }

    public void StartSliding()
    {
        _isSlidingIn = true;
        _time = 0f;
    }
}