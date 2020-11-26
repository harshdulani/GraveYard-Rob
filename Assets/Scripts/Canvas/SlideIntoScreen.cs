using System;
using System.Collections;
using UnityEngine;

public enum SlideInDirection
{
    fromLeft,
    fromRight,
    fromTop,
    fromBottom
}

public class SlideIntoScreen : MonoBehaviour
{
    public SlideInDirection mySlideInDirection;

    private Vector3 _initialLocalPosition, _newLocalPosition;
    private float _time, _endTime;
    private float _multiplier, _sign;
    private bool _isSlidingIn;

    private RectTransform _rectTransform;
    [SerializeField] AnimationCurve _slideOverTimeCurve;
    
    private void OnEnable()
    {
        GameFlowEvents.current.gameplayStart += StartSliding;
    }

    private void OnDisable()
    {
        GameFlowEvents.current.gameplayStart -= StartSliding;
    }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _initialLocalPosition = _rectTransform.localPosition;
        _endTime = _slideOverTimeCurve.keys[_slideOverTimeCurve.keys.Length - 1].time;

        switch (mySlideInDirection)
        {
            case SlideInDirection.fromLeft:
            case SlideInDirection.fromRight:
                _rectTransform.localPosition = _newLocalPosition = new Vector3(_initialLocalPosition.x * 2, _initialLocalPosition.y, _initialLocalPosition.z);
                break;
            case SlideInDirection.fromTop:
            case SlideInDirection.fromBottom:
                _rectTransform.localPosition = _newLocalPosition = new Vector3(_initialLocalPosition.x, _initialLocalPosition.y * 2, _initialLocalPosition.z);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        print("new pos = " + _rectTransform.localPosition);
    }

    private void Update()
    {
        if(_isSlidingIn)
            SlideIn();
    }

    private void SlideIn()
    {
        _time += Time.deltaTime;

        _multiplier = _slideOverTimeCurve.Evaluate(_time);

        switch (mySlideInDirection)
        {
            case SlideInDirection.fromLeft:
            case SlideInDirection.fromRight:
                _rectTransform.localPosition = new Vector3(_newLocalPosition.x - _initialLocalPosition.x * _multiplier, _newLocalPosition.y, _newLocalPosition.z);
                break;
            case SlideInDirection.fromTop:
            case SlideInDirection.fromBottom:
                _rectTransform.localPosition = new Vector3(_newLocalPosition.x, _newLocalPosition.y + _initialLocalPosition.y * _multiplier, _newLocalPosition.z);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (_time >= _endTime)
            _isSlidingIn = false;
    }

    private void StartSliding()
    {
        _isSlidingIn = true;
        _time = 0f;
    }
}