using System.Collections;
using UnityEngine;

public class MovementInput : MonoBehaviour
{
    public static MovementInput current;
    
    [Header("General")]
    public bool playerHasControl = true;
    [Tooltip("Must also change Animator Blend tree to corresponding change")]
    public bool shouldFaceTowardMouse = true;
    
    [Header("Movement")]
    public float movementSpeed;
    public bool isWalking = true;
    public Vector3 desiredMovementDirection;

    private float _originalMovementSpeed;
    
    [Header("Rotation")]
    public float rotationSlerpSpeed;
    public float allowPlayerRotationSpeed;

    [Header("Rolling And Grounding")]
    public float rollSpeed = 7.5f; //is a good value for vertical jumps
    public bool isJumping = false;
    public bool isGrounded;
    private bool _jumpDone = false;

    private Vector3 _groundingVector;

    [Header("Diagonal StrafeRunning")]
    [Range(0f, 1f)]
    public float rotationStrafeFactor;
    [Range(1f, 2f)]
    public float speedLimitStrafeRunning = 1.35f;

    [Header("Side Strafe Forward Movement threshold")] [Range(0.25f, 1f)]
    public float sideStrafeThreshold = 0.25f;

    [Header("Wheelbarrow modifiers")]
    public bool blockStrafing;
    public bool blockWalkBack, blockJumping;

    private float _speed;
    private float _inputX, _inputZ;
    private Vector3 _forward, _right;

    //animator hashes for performance++
    private static readonly int SpeedHash = Animator.StringToHash("inputMagnitude");
    private static readonly int ValXHash = Animator.StringToHash("valX");
    private static readonly int ValZHash = Animator.StringToHash("valZ");
    private static readonly int IsMovingHash= Animator.StringToHash("isMoving");
    private static readonly int StartJumpHash = Animator.StringToHash("startJump");    
    private static readonly int LandingFromFence = Animator.StringToHash("landingFromFence");

    //components
    private Transform _cam;
    private static Animator _animator;
    private CharacterController _controller;

    private void Awake()
    {
        if (current == null)
            current = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _cam = Camera.main.transform;
        _animator = GetComponent<Animator>();

        _originalMovementSpeed = movementSpeed;
    }

    private void Update()
    {
        isGrounded = _controller.isGrounded;
        
        if(!GameStats.current.isGamePlaying) return;
        
        if(playerHasControl)
        {
            if (Input.GetButtonDown("Jump") && !isJumping)
            {
                if(!blockJumping)
                    StartJump();
            }
            isWalking = Input.GetKey(KeyCode.LeftShift);
            InputMagnitude();
        }
        else if(!isGrounded)
            GroundPlayer();
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
        _animator.SetFloat(ValXHash, _inputX, 0.1f, Time.deltaTime * 2f);
        _animator.SetFloat(ValZHash, _inputZ, 0.1f, Time.deltaTime * 2f);

        _animator.SetFloat(SpeedHash, _speed);

        if (_speed > allowPlayerRotationSpeed)
        {
            if(shouldFaceTowardMouse && !isJumping)
                MoveFacingMouseForward();
            else
                MoveFacingMovementDirection(isJumping ? 5f : 1f);
        }
        else
        {
            _animator.SetBool(IsMovingHash, false);
        }
    }

    private void MoveFacingMouseForward()
    {
        var rotateCamForward = _forward = _cam.forward;
        _right = _cam.right;

        rotateCamForward.y = _forward.y = 0f;
        _right.y = 0f;

        _forward.Normalize();
        _right.Normalize();
        
        if(blockWalkBack && _inputZ < 0f)
            desiredMovementDirection = Vector3.zero;
        else
            desiredMovementDirection = _forward * (_inputZ * movementSpeed);

        if(!blockStrafing)
        {
            if (_inputZ >= -sideStrafeThreshold)
            {
                desiredMovementDirection += _right * (_inputX * movementSpeed);
                desiredMovementDirection *= Mathf.Clamp(_speed, -speedLimitStrafeRunning, speedLimitStrafeRunning);
                if (_speed > 1.2f)
                {
                    rotateCamForward += _right * ((_inputX) * (_speed - (Mathf.Sign(_speed) * (_speed - rotationStrafeFactor))));
                }
            }
        }

        if (!desiredMovementDirection.Equals(Vector3.zero))
        {
            if(_speed >= 1.25f && _inputZ >= 0f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMovementDirection), rotationSlerpSpeed);
            else
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotateCamForward), rotationSlerpSpeed);
            _animator.SetBool(IsMovingHash, true);
        }
        
        if (!isGrounded)
        {
            if (transform.position.y >= 0.15f)
                desiredMovementDirection.y = -transform.position.y * 2f;
        }

        //not using delta time made the movement speed dependent on screen size (easy render high fps)
        _controller.Move(desiredMovementDirection * Time.deltaTime);
    }

    private void MoveFacingMovementDirection(float lerpMultiplier = 1f)
    {
        _forward = _cam.forward;
        _right = _cam.right;

        _forward.y = 0f;
        _right.y = 0f;

        _forward.Normalize();
        _right.Normalize();

        desiredMovementDirection = _forward * (_inputZ * movementSpeed) + _right * (_inputX * movementSpeed);

        if (isWalking)
            desiredMovementDirection *= 0.15f;

        if (!desiredMovementDirection.Equals(Vector3.zero))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMovementDirection),
                rotationSlerpSpeed * lerpMultiplier);
            _animator.SetBool(IsMovingHash, true);
        }

        if (isJumping && !_animator.GetBool(LandingFromFence))
        {
            desiredMovementDirection *= rollSpeed;
            _jumpDone = true;
            playerHasControl = false;
        }

        if (_jumpDone)
        {
            if (!isGrounded)
            {
                if (desiredMovementDirection.y >= 0.5f)
                    desiredMovementDirection.y = -desiredMovementDirection.y;
            }
            else
            {
                _jumpDone = false;
                GiveBackMovementControl();
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

    private void GroundPlayer()
    {
        if (transform.position.y >= 0.15f)
            _groundingVector.y = -transform.position.y * 2f;
        
        _controller.Move(_groundingVector * Time.deltaTime);
    }
    
    private void StartJump()
    {
        if(!PlayerEvents.current.InvokeStaminaChange(PlayerStats.main.jumpStaminaCost)) return;
        //go ahead with execution (jump) if there is enough stamina
        
        _animator.SetTrigger(StartJumpHash);
        isJumping = true;
        _jumpDone = false;
        TakeAwayMovementControl();

        if(!desiredMovementDirection.Equals(Vector3.zero))
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMovementDirection), rotationSlerpSpeed * 2f);
        
        rotationSlerpSpeed *= 0.1f;
    }
    
    public void EndJump()
    {
        isJumping = false;
        _jumpDone = true;
        GiveBackMovementControl();
        rotationSlerpSpeed /= 0.1f;
    }
    
    public void TakeAwayMovementControl()
    {
        playerHasControl = false;
        desiredMovementDirection = Vector3.zero;
        _animator.SetFloat(SpeedHash, 0f);
    }
    
    public void GiveBackMovementControl()
    {
        playerHasControl = true;
    }

    public void SlowDownMovement(float multiplier)
    {
        movementSpeed = _originalMovementSpeed * multiplier;
    }

    public void RestoreMovement()
    {
        movementSpeed = _originalMovementSpeed;
    }
    
    #region  legacy coroutine code for taking away movement control

    public void TakeAwayMovementControlFor(float seconds)
    {
        playerHasControl = false;
        desiredMovementDirection = Vector3.zero;
        _animator.SetFloat(SpeedHash, 0f);
        StartCoroutine(TakingAwayControl(seconds));
    }
    
    private IEnumerator TakingAwayControl(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        playerHasControl = true;
    }

    #endregion
}