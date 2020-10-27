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

    private Ray ray;
    private Vector3 position;
    private bool rotatingToPosition = false;
    private Vector3 desiredMovementDirection;

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

    private Animator anim;
    private MovementInput movementInput;
    private Camera cam;

    
    private PlayerWeaponController _playerWeaponController;
    private TargetAreaController _targetAreaController;

    private void Start()
    {
        _targetAreaController = GameObject.FindGameObjectWithTag("Target").GetComponent<TargetAreaController>();
        _playerWeaponController = GetComponentInChildren<PlayerWeaponController>();
        anim = GetComponent<Animator>();
        movementInput = GetComponent<MovementInput>();
        cam = Camera.main;
    }

    private void Update()
    {
        if (!isAttacking)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if(!isDiggingComplete)
                    if (_allowedToDig)
                    {
                        StartDigging();
                    }
                else
                {
                    // Reset ray with new mouse position
                    ray = cam.ScreenPointToRay(Input.mousePosition);

                    foreach (var hit in Physics.RaycastAll(ray))
                        position = hit.point;
                    
                    StartAttack();

                    //definitely replace this with on animation end
                }
            }
        }
        
        if(rotatingToPosition)
        {
            Rotate(position);
        }
    }

    private void StartAttack()
    {
        anim.SetTrigger(Attack1Hash);

        isAttacking = true;
        _playerWeaponController.shouldGiveHit = true;
        movementInput.TakeAwayMovementControl();
        rotatingToPosition = true;
    }

    public void CompleteAttack()
    {
        movementInput.GiveBackMovementControl();
        
        isAttacking = false;
        _playerWeaponController.shouldGiveHit = false;
        rotatingToPosition = false;
        position = Vector3.zero;
    }

    private void SwapWeapon()
    {
        anim.SetTrigger(CycleWeapon);
        isAttacking = true;
    }

    public void CompleteWeaponSwap()
    {
        shovel.SetActive(_allowedToDig);
        weapon.SetActive(!_allowedToDig);
        isAttacking = false;
    }
    
    private void StartDigging()
    {
        anim.SetTrigger(ShouldDig);
        movementInput.TakeAwayMovementControl();
        
        _targetAreaController.TargetGiveHit();
        
        isAttacking = true;
    }

    public void CompleteDigging()
    {
        movementInput.GiveBackMovementControl();
        isAttacking = false;
    }

    private void Rotate(Vector3 position)
    {
        //if using skyrim camera, send v3.zero instead of your camera position
        if (position.Equals(Vector3.zero))
            desiredMovementDirection = transform.position - cam.transform.position;
        else
            desiredMovementDirection = position - transform.position;

        desiredMovementDirection.y = 0f;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMovementDirection), 0.2f);
    }
}
