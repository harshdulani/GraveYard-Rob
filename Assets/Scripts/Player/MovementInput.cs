using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovementInput : MonoBehaviour
{
    public float movementSpeed;
    public bool isWalking = true;
    public float rotationSlerpSpeed;

    //rotation
    public Vector3 desiredMovementDirection;
    public bool playerHasControl = true;    
    public float allowPlayerRotationSpeed;

    //jumping
    public float jumpSpeed = 7.5f;
    public bool doJump = false;

    //grounding
    public bool isGrounded;
    private bool _jumpDone = false;

    private float _speed;
    private float _inputX, _inputZ;
    private Vector3 _forward, _right;

    //animator hashes for performance++
    private int _speedHash, _valXHash, _valZHash, _isMovingHash, _startJumpHash;

    //componentss
    private Camera _cam;
    private static Animator _animator;
    private CharacterController _controller;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _cam = Camera.main;
        _animator = GetComponent<Animator>();

        _speedHash = Animator.StringToHash("inputMagnitude");
        _valXHash = Animator.StringToHash("valX");
        _valZHash = Animator.StringToHash("valZ");
        _isMovingHash = Animator.StringToHash("isMoving");
        _startJumpHash = Animator.StringToHash("startJump");
    }

    private void Update()
    {
        isGrounded = _controller.isGrounded;

        if(playerHasControl)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _animator.SetTrigger(_startJumpHash);
                doJump = true;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isWalking = true;
            }
            else
                isWalking = false;
            InputMagnitude();
        }
    }

    private void InputMagnitude()
    {
        _inputX = Input.GetAxis("Horizontal");
        _inputZ = Input.GetAxis("Vertical");
        
        _speed = new Vector2(_inputX, _inputZ).sqrMagnitude;

        if (isWalking)
        {
            _speed = Mathf.Clamp(_speed, -0.5f, 0.5f);
            _inputX = Mathf.Clamp(_inputX, -0.75f, 0.75f);
        }

        //sending input values to animator
        //the third value is damping, set for blending on keyboards
        _animator.SetFloat(_valXHash, _inputX, 0.1f, Time.deltaTime * 2f);
        _animator.SetFloat(_valZHash, _inputZ, 0.1f, Time.deltaTime * 2f);

        _animator.SetFloat(_speedHash, _speed);

        if (_speed > allowPlayerRotationSpeed)
        {
            PlayerMoveAndRotate();
        }
        else
        {
            _animator.SetBool(_isMovingHash, false);
        }
    }

    private void PlayerMoveAndRotate()
    {
        _forward = _cam.transform.forward;
        _right = _cam.transform.right;

        _forward.y = 0f;
        _right.y = 0f;

        _forward.Normalize();
        _right.Normalize();

        desiredMovementDirection = _forward * (_inputZ * movementSpeed) + _right * (_inputX * movementSpeed);

        if (isWalking)
            desiredMovementDirection *= 0.15f;


        if (!desiredMovementDirection.Equals(Vector3.zero))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMovementDirection), rotationSlerpSpeed);
            _animator.SetBool(_isMovingHash, true);
        }

        if (doJump)
        {
            desiredMovementDirection += (Vector3.up * jumpSpeed);
            doJump = false;
            _jumpDone = true;
        }

        if(_jumpDone)
        {
            if (!isGrounded)
            {
                if (desiredMovementDirection.y >= 0.5f)
                    desiredMovementDirection.y = -desiredMovementDirection.y;
            }
            else
            {
                _jumpDone = false;
            }
        }

        if (!isGrounded)
        {
            if (transform.position.y >= 0.15f)
                desiredMovementDirection.y = -transform.position.y * 2f;
        }

        //not using delta time made the movement speed dependent on screen size (easy render high fps)
        _controller.Move(desiredMovementDirection * Time.deltaTime);
    }

    public void TakeAwayMovementControlFor(float seconds)
    {
        playerHasControl = false;
        desiredMovementDirection = Vector3.zero;
        _animator.SetFloat(_speedHash, 0f);
        StartCoroutine(TakingAwayControl(seconds));
    }

    public void TakeAwayMovementControl()
    {
        playerHasControl = false;
        desiredMovementDirection = Vector3.zero;
        _animator.SetFloat(_speedHash, 0f);
    }
    
    public void GiveBackMovementControl()
    {
        playerHasControl = true;
    }

    private IEnumerator TakingAwayControl(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        playerHasControl = true;
    }
}
