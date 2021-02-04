using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyDiagonalMovement : MonoBehaviour
{
    public int minJumpsPerMovement = 3, jumpVariability = 2;
    
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
    private bool _shouldRotate, _shouldJump, _hasLanded = true, _shouldStartMoving;
    
    private Rigidbody _rigidbody;
    private ScreenShakes _shakes;
    
    private Animator _animator;
    private static readonly int ShouldRanged = Animator.StringToHash("shouldRanged");

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
    }

    private void Update()
    {
        if (_shouldStartMoving)
            StartCoroutine(Movement(Random.Range(minJumpsPerMovement, minJumpsPerMovement + jumpVariability)));
        
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

    private IEnumerator Movement(int times)
    {
        _forceMultiplier = Random.Range(jumpForceMultiplierMin, jumpForceMultiplierMax);
        
        _shouldStartMoving = false;
        
        //nai toh saath mein attack karte hain
        times += Random.Range(0, 4);
        
        while (0 <= times)
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
            times--;
        }

        //start infernal attack
        StartCoroutine(WaitAndBeginAgain(Instantiate(inferno, parent: null)));
    }

    private IEnumerator WaitAndBeginAgain(GameObject instance)
    {
        _animator.SetBool(ShouldRanged, true);
        
        casterVFX.Play();
        yield return new WaitForSeconds(instance.GetComponent<InfernalAttackController>().followPlayerBeforeAttackTime + 1f); //this is equal to the duration of the attack indicator
        
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
