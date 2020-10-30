using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Melee Combat")] [SerializeField]
    private GameObject weapon;
    public bool isAttacking = false;

    private Ray _ray;
    private Vector3 _position;
    private bool _rotatingToPosition = false;
    private Vector3 _desiredMovementDirection;
    private bool _shouldRotateToRaycastHit;

    [Header("Digging Grave")] [SerializeField]
    private GameObject shovel;

    public bool isDiggingComplete;

    private bool _allowedToDig;

    public bool IsAllowedToDig
    {
        get => _allowedToDig;
        set
        {
            _allowedToDig = value;
            SwapWeapon();
        }
    }

    private static readonly int Attack1Hash = Animator.StringToHash("attack1");
    private static readonly int ShouldDig = Animator.StringToHash("shouldDig");
    private static readonly int CycleWeapon = Animator.StringToHash("cycleWeapon");

    private Animator _anim;
    private MovementInput _movementInput;
    private Camera _cam;

    
    private PlayerWeaponController _playerWeaponController;
    private TargetAreaController _targetAreaController;

    private void Start()
    {
        _targetAreaController = GameObject.FindGameObjectWithTag("Target").GetComponent<TargetAreaController>();
        _playerWeaponController = GetComponentInChildren<PlayerWeaponController>();
        _anim = GetComponent<Animator>();
        _movementInput = GetComponent<MovementInput>();
        _cam = Camera.main;

        _shouldRotateToRaycastHit = !_movementInput.shouldFaceTowardMouse;
    }

    private void Update()
    {
        if (!isAttacking)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (_allowedToDig)
                {
                    if (!isDiggingComplete)
                    {
                        StartDigging();
                    }
                }
                else
                {
                    if (_shouldRotateToRaycastHit)
                    {
                        // Reset ray with new mouse position
                        _ray = _cam.ScreenPointToRay(Input.mousePosition);

                        foreach (var hit in Physics.RaycastAll(_ray))
                            _position = hit.point;
                    }

                    StartAttack();
                }
            }
        }
        
        if(_rotatingToPosition)
        {
            Rotate(_position);
        }
    }

    private void StartAttack()
    {
        _anim.SetTrigger(Attack1Hash);

        isAttacking = true;
        _playerWeaponController.shouldGiveHit = true;
        _movementInput.TakeAwayMovementControl();
        
        if (_shouldRotateToRaycastHit)
            _rotatingToPosition = true;
    }

    public void CompleteAttack()
    {
        _movementInput.GiveBackMovementControl();
        
        isAttacking = false;
        _playerWeaponController.shouldGiveHit = false;

        if (_shouldRotateToRaycastHit)
        {
            _rotatingToPosition = false;
            _position = Vector3.zero;
        }
    }

    private void SwapWeapon()
    {
        print("swap weapon started");
        _anim.SetTrigger(CycleWeapon);
        isAttacking = true;
    }

    public void CompleteWeaponSwap()
    {
        print("swap weapon ended, allowedtodig = " + _allowedToDig);
        shovel.SetActive(_allowedToDig);
        weapon.SetActive(!_allowedToDig);
        isAttacking = false;
    }
    
    private void StartDigging()
    {
        print("digging started");
        _anim.SetTrigger(ShouldDig);
        _movementInput.TakeAwayMovementControl();
        
        _targetAreaController.TargetGiveHit();
        
        isAttacking = true;
        _allowedToDig = false;
    }

    public void CompleteDigging()
    {
        print("digging ended");
        _movementInput.GiveBackMovementControl();
        isAttacking = false;
        _allowedToDig = true;
    }

    private void Rotate(Vector3 position)
    {
        //if using skyrim camera, send v3.zero instead of your camera position
        if (position.Equals(Vector3.zero))
            _desiredMovementDirection = transform.position - _cam.transform.position;
        else
            _desiredMovementDirection = position - transform.position;

        _desiredMovementDirection.y = 0f;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_desiredMovementDirection), 0.2f);
    }
}
