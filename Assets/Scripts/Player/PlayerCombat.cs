using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Melee Combat")] [SerializeField]
    private GameObject weapon;
    public float waitBeforeAttacking = 0.75f;
    public bool isAttacking = false;

    private Ray ray;
    private Vector3 position;
    private bool rotatingToPosition = false;
    private Vector3 desiredMovementDirection;

    [Header("Digging Grave")] [SerializeField]
    private GameObject shovel;
    public float waitBeforeDigging = 2.5f;

    private bool _allowedToDig;

    public bool AllowedToDig
    {
        get => _allowedToDig;
        set
        {
            _allowedToDig = value;
            StartCoroutine(SwapWeapon());
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
                if (_allowedToDig)
                {
                    StartCoroutine(StartDigging());
                }
                else
                {
                    // Reset ray with new mouse position
                    ray = cam.ScreenPointToRay(Input.mousePosition);

                    foreach (var hit in Physics.RaycastAll(ray))
                        position = hit.point;  

                    //refactor this to hashid
                    anim.SetTrigger(Attack1Hash);

                    isAttacking = true;
                    GetComponentInChildren<PlayerWeaponController>().shouldGiveHit = true;
                    movementInput.TakeAwayMovementControlFor(waitBeforeAttacking);
                    rotatingToPosition = true;

                    //definitely replace this with on animation end
                    StartCoroutine(WaitBeforeAllowingAttack());
                }
            }
        }
        
        if(rotatingToPosition)
        {
            Rotate(position);
        }
    }

    private IEnumerator SwapWeapon()
    {
        anim.SetTrigger(CycleWeapon);
        
        isAttacking = true;
        yield return new WaitForSeconds(1.5f);
        shovel.SetActive(_allowedToDig);
        weapon.SetActive(!_allowedToDig);
        isAttacking = false;
    }
    
    private IEnumerator StartDigging()
    {
        anim.SetTrigger(ShouldDig);
        movementInput.TakeAwayMovementControlFor(waitBeforeDigging);

        _targetAreaController.TargetGiveHit();
        
        isAttacking = true;
        yield return new WaitForSeconds(waitBeforeDigging);
        isAttacking = false;
    }

    private IEnumerator WaitBeforeAllowingAttack()
    {
        yield return new WaitForSeconds(waitBeforeAttacking);

        isAttacking = false;
        _playerWeaponController.shouldGiveHit = false;
        rotatingToPosition = false;
        position = Vector3.zero;
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
