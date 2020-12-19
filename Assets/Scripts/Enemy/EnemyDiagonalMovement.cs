using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyDiagonalMovement : MonoBehaviour
{
    public float rotationLerpSpeed = 0.1f;

    public float forwardForce = 10f, upwardForce = 10f;
    
    private readonly float[] _availableAngles = new float[] {45, 135, 225, 315};

    private float _targetAngle, _currentAngle;
    private bool _shouldRotate, _shouldJump;

    private Rigidbody _rigidbody;

    private void Start()
    {
       _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
            CalculateAndMove();
        
        if(_shouldRotate)
            Rotate();
        if (_shouldJump)
        {
            //try adding better jumping from floor is lava
            _rigidbody.AddForce(transform.forward * forwardForce + transform.up * upwardForce);
            _shouldJump = false;
        }
    }

    private void CalculateAndMove()
    {
        //calculate
        _targetAngle = _availableAngles[Random.Range(0, 3)];
        _shouldRotate = true;
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
}
