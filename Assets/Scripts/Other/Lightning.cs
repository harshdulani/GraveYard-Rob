using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animation))]
public class Lightning : MonoBehaviour
{
    public List<float> waitTimes = new List<float>();

    private List<WaitForSeconds> _waiters = new List<WaitForSeconds>();

    private bool _isFirstThunder = true;
    private int CurrentAnimation
    {
        get => _currentAnimation;
        set => _currentAnimation = value % _totalStates;
    }
    private int _currentAnimation, _totalStates;

    private Animation _animation;
    private List<AnimationState> _states = new List<AnimationState>();

    private void OnEnable()
    {
        GameFlowEvents.current.updateObjective += OnUpdateObjective;
    }

    private void OnDisable()
    {
        GameFlowEvents.current.updateObjective -= OnUpdateObjective;
    }

    private void Start()
    {
        _animation = GetComponent<Animation>();
        foreach (AnimationState state in _animation)
            _states.Add(state);
        _totalStates = _states.Count;

        foreach (var time in waitTimes)
        {
            _waiters.Add(new WaitForSeconds(time));
        }
    }

    private IEnumerator StartThundering()
    {
        if (_isFirstThunder)
        {
            _isFirstThunder = false;
            _animation.Play(_states[0].name);
        }

        while (true)
        {
            yield return _waiters[Random.Range(0, _waiters.Count)];

            //TODO: Add thundering sounds as animation events
            _animation.Play(_states[CurrentAnimation++].name);
        }
    }

    private void OnUpdateObjective()
    {
        if(!_isFirstThunder) return;
        StartCoroutine(StartThundering());
    }
}