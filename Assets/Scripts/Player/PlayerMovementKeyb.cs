using UnityEngine;

public class PlayerMovementKeyb : MonoBehaviour
{
    public static bool IsRunning = false;

    private static Animator _animator;

    private float _inputX, _inputZ;
    private Vector3 _direction;

    //animator hashes for performance++
    private int _speedHash, _valXHash, _valZHash, _isMovingHash, _startJumpHash;

    //ease in lerping movement
    public float currentMovementSpeed, desiredMovementSpeed;

    private float _lerpTime = 0.1f;

    //lerping of rotation
    public float rotationLerpSpeed = 0.1f;

    private float _targetAngle, _currentAngle;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _speedHash = Animator.StringToHash("inputMagnitude");
        _valXHash = Animator.StringToHash("valX");
        _valZHash = Animator.StringToHash("valZ");
        _isMovingHash = Animator.StringToHash("isMoving");
        _startJumpHash = Animator.StringToHash("startJump");
    }

    private void Update()
    {
        _inputX = Input.GetAxis("Horizontal");
        _inputZ = Input.GetAxis("Vertical");
        _direction = new Vector3(_inputX, 0f, _inputZ);

        if (Input.GetButtonDown("Jump"))
        {
            _animator.SetTrigger(_startJumpHash); 
            //RotateTowardsMouse.shouldRotate = false;
        }

        if (_direction.Equals(Vector3.zero))     //or isn't shooting
        {
            IsRunning = false;
            _animator.SetBool(_isMovingHash, false);
            currentMovementSpeed = 0f;
        }
        else
        {
            IsRunning = true;
            //RotateTowardsMouse.shouldRotate = true;
            _animator.SetBool(_isMovingHash, true);
            _animator.SetFloat(_valXHash, _inputX);
            _animator.SetFloat(_valZHash, _inputZ);            

            //PLAYER ROTATION

            _targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;

            _currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, _targetAngle, rotationLerpSpeed);

            transform.rotation = Quaternion.AngleAxis(_currentAngle, transform.up);

            //PLAYER MOVEMENT

            desiredMovementSpeed = new Vector2(_inputX, _inputZ).sqrMagnitude;

            currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, desiredMovementSpeed, _lerpTime);
            //idk what this does
            _animator.SetFloat(_speedHash, desiredMovementSpeed);

            GetComponent<CharacterController>().Move(_direction * Time.deltaTime * currentMovementSpeed);
        }
    }
}
