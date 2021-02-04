﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyDiagonalMovement : MonoBehaviour
{
    public float minTimeBeforeAttack = 5f, attackVariability = 7f;
    
    public float forwardForce = 500f, upwardForce = 550f;
    public float jumpForceMultiplierMin = 0.6f, jumpForceMultiplierMax = 1.5f; 
    public float fallMultiplier = 2.5f, jumpMultiplier = 2f; //better jumping

    public float rotationLerpSpeed = 0.1f;

    public float timeBetweenJumps = 0.5f;
    
    //[HideInInspector]
    public List<int> availableAngles = new List<int> {45, 135, 225, 315};

    [Header("Infernal Attack")] 
    public GameObject inferno;
    public ParticleSystem casterVFX;

    private float _forceMultiplier;
    
    private int _targetAngle = 0;
    private float _currentAngle;
    public bool _shouldRotate, _shouldJump, _hasLanded = true, _shouldStartMoving;
    private bool _waitingToAttack;

    public float _countdownForStuck = 2f;
    private bool _isStuck;
    
    private Rigidbody _rigidbody;
    private ScreenShakes _shakes;
    
    private Animator _animator;
    private static readonly int ShouldRanged = Animator.StringToHash("shouldRanged");

    private WaitForSeconds countdownWait;

    private void OnEnable()
    {
        PlayerEvents.current.playerDeath += OnPlayerDeath;
    }
    
    private void OnDisable()
    {
        PlayerEvents.current.playerDeath -= OnPlayerDeath;
    }

    private void Start()
    {
       _rigidbody = GetComponent<Rigidbody>();
       _animator = GetComponent<Animator>();
       _shakes = GetComponent<ScreenShakes>();
       
       countdownWait = new WaitForSeconds(_countdownForStuck);
       StartCoroutine(CheckForStuck());
    }

    private void Update()
    {
        if (_shouldStartMoving)
            StartCoroutine(Movement());
        
        if(_shouldRotate)
            Rotate();
        
        if (_shouldJump)
        {
            _rigidbody.AddForce((transform.forward * forwardForce + transform.up * upwardForce) * _forceMultiplier);
            _shouldJump = false;
            _hasLanded = false;
        }
        //for better jumping
        if (_rigidbody.velocity.y < -1f) //falling
        {
            _rigidbody.velocity += Vector3.up * (Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
        }
        else if (_rigidbody.velocity.y > 1f) //"rising"
        {
            _rigidbody.velocity += Vector3.up * (Physics.gravity.y * (jumpMultiplier - 1) * Time.deltaTime);
        }
    }

    private void CalculateRotation()
    {
        //calculate
        var keepCalculating = true;
        var lastUsedAngle = _targetAngle;
        while (keepCalculating)
        {
            if (availableAngles.Count > 1)
            {
                _targetAngle = availableAngles[Random.Range(0, availableAngles.Count)];
                keepCalculating = (lastUsedAngle == _targetAngle);
            }
            else
            {
                _targetAngle = availableAngles[0];
                keepCalculating = false;
            }
        }
        _shouldRotate = true;
    }

    private IEnumerator Movement()
    {
        _forceMultiplier = Random.Range(jumpForceMultiplierMin, jumpForceMultiplierMax);
        
        _shouldStartMoving = false;
        StartCoroutine(AttackCountdown(Random.Range(minTimeBeforeAttack, minTimeBeforeAttack + attackVariability)));
        
        while (!_waitingToAttack)
        {
            if (_hasLanded && !_shouldRotate)
            {
                yield return new WaitForSeconds(timeBetweenJumps);
                CalculateRotation();
            }
            else
            {
                while(!_hasLanded || _shouldRotate)
                    yield return new WaitForEndOfFrame();
            }
        }
        
        //start infernal attack
        StartCoroutine(WaitAndBeginAgain(Instantiate(inferno, parent: null)));
        _waitingToAttack = false;
    }

    private IEnumerator AttackCountdown(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _waitingToAttack = true;
    }

    private IEnumerator CheckForStuck()
    {
        while (true)
        {
            yield return countdownWait;
            if (_isStuck)
            {
                _hasLanded = true;
                _isStuck = false;
            }
            else
            {
                if (!_hasLanded && !_shouldRotate)
                {
                    _isStuck = true;
                }
            }
        }
    }

    private IEnumerator WaitAndBeginAgain(GameObject instance)
    {
        _animator.SetBool(ShouldRanged, true);
        
        casterVFX.Play();
        yield return new WaitForSeconds(instance.GetComponent<InfernalAttackController>().followPlayerBeforeAttackTime + 2f); //this is equal to the duration of the attack indicator
        
        _shouldStartMoving = true;
        _animator.SetBool(ShouldRanged, false);
        casterVFX.Stop();
    }
    
    private void Rotate()
    {
        if (Mathf.Abs(_currentAngle - _targetAngle) >= 1f)
        {
            _currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, _targetAngle, rotationLerpSpeed);

            transform.eulerAngles = Vector3.up * _currentAngle;
        }
        else
        {
            _shouldRotate = false;
            _shouldJump = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(!other.gameObject.CompareTag("Ground")) return;

        _hasLanded = true;
        _shakes.Light();
    }

    public void StartMoving()
    {
        _shouldStartMoving = true;
    }
    
    private void OnPlayerDeath()
    {
        StopAllCoroutines();
    }
}
