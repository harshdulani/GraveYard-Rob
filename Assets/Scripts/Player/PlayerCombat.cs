using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Melee Combat")] 
    public GameObject weapon;
    public GameObject weaponOnBack;
    public bool isAttacking;
    public static AttackType currentAttackType;
    public ParticleSystem slash;
    public float slashSpeed = .5f;
    
    private Ray _ray;
    private Vector3 _position;
    private bool _rotatingToPosition;
    private Vector3 _desiredMovementDirection;
    private bool _shouldRotateToRaycastHit;

    [Header("Digging Grave")] 
    public GameObject shovel;
    public GameObject shovelOnBack;
    public bool isDiggingComplete;
    public TargetAreaController _targetAreaController;

    private bool _allowedToDig;

    private ScreenShakes _shakes;
    
    public bool IsAllowedToDig
    {
        get => _allowedToDig;
        set
        {
            _allowedToDig = value;
            SwapWeapon();
        }
    }

    [Header("Audio")]
    public List<AudioClip> lightAttackSounds;
    public List<AudioClip> heavyAttackSounds, digSounds;

    private AudioSource _audio;
    
    private Animator _anim;
    private MovementInput _movementInput;
    private Camera _cam;

    private PlayerWeaponController _playerWeaponController;

    private static readonly int Attack1Hash = Animator.StringToHash("attack1");
    private static readonly int Attack2Hash = Animator.StringToHash("attack2");
    private static readonly int ShouldDig = Animator.StringToHash("shouldDig");
    private static readonly int CycleWeapon = Animator.StringToHash("cycleWeapon");

    private void Start()
    {
        _playerWeaponController = GetComponentInChildren<PlayerWeaponController>();
        _anim = GetComponent<Animator>();
        _movementInput = MovementInput.current;
        _shakes = GetComponent<ScreenShakes>();
        _cam = Camera.main;
        _audio = GetComponent<AudioSource>();

        _shouldRotateToRaycastHit = !_movementInput.shouldFaceTowardMouse;

        //didnt wanna set this value and execute the setter of the property
        _allowedToDig = shovel.activeSelf; 
        
        shovelOnBack.SetActive(!IsAllowedToDig);
        weaponOnBack.SetActive(!IsAllowedToDig);

        var mainModule = slash.main;
        mainModule.simulationSpeed = slashSpeed;
    }

    private void Update()
    {
        if(!GameStats.current.isGamePlaying) return;
        if(!MovementInput.current.playerHasControl) return;
        if (!isAttacking && !MovementInput.current.isJumping)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (IsAllowedToDig)
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

                    currentAttackType = AttackType.LightAttack;
                    StartAttack(currentAttackType);
                }
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                if (!IsAllowedToDig)
                {
                    if (_shouldRotateToRaycastHit)
                    {
                        // Reset ray with new mouse position
                        _ray = _cam.ScreenPointToRay(Input.mousePosition);

                        foreach (var hit in Physics.RaycastAll(_ray))
                            _position = hit.point;
                    }


                    currentAttackType = AttackType.HeavyAttack;
                    StartAttack(currentAttackType);
                }
            }
        }
        
        if(_rotatingToPosition)
        {
            Rotate(_position);
        }
    }

    private void StartAttack(AttackType type)
    {
        if (type == AttackType.LightAttack)
        {
            if (!PlayerEvents.current.InvokeStaminaChange(PlayerStats.main.lightAttackStaminaCost)) return;
            
            _anim.SetTrigger(Attack1Hash);
            _audio.PlayOneShot(lightAttackSounds[Random.Range(0, 2)]);
        }
        else if (type == AttackType.HeavyAttack)
        {
            if(!PlayerEvents.current.InvokeStaminaChange(PlayerStats.main.heavyAttackStaminaCost)) return;
            
            _anim.SetTrigger(Attack2Hash);
            _audio.PlayOneShot(heavyAttackSounds[Random.Range(0, 2)]);
        }
        
        PlayerEvents.current.InvokePlayerCombatStrikeStart();

        isAttacking = true;
        //_playerWeaponController.shouldGiveHit = true;
        _movementInput.TakeAwayMovementControl();
        
        if (_shouldRotateToRaycastHit)
            _rotatingToPosition = true;
    }

    public void CanActuallyHit()
    {
        slash.Play();
        //this is an animation event, so that collisions before the sword looks like its attacking dont count
        //hence the players that were bound to get hit regardless, get hit when it matters
        
        //this is for both light and heavy attacks
        _playerWeaponController.shouldGiveHit = true;
    }

    public void CompleteAttack()
    {
        PlayerEvents.current.InvokePlayerCombatStrikeEnd();
        _movementInput.GiveBackMovementControl();
        
        isAttacking = false;
        _playerWeaponController.shouldGiveHit = false;
        _playerWeaponController.ClearAttackedEnemies();

        if (_shouldRotateToRaycastHit)
        {
            _rotatingToPosition = false;
            _position = Vector3.zero;
        }
    }

    public void SwapWeapon()
    {
        _anim.SetTrigger(CycleWeapon);
        isAttacking = true;
    }

    public void CompleteWeaponSwap()
    {
        shovel.SetActive(IsAllowedToDig);
        weapon.SetActive(!IsAllowedToDig);
        shovelOnBack.SetActive(!IsAllowedToDig);
        weaponOnBack.SetActive(IsAllowedToDig);
        isAttacking = false;
    }
    
    private void StartDigging()
    {
        _anim.SetTrigger(ShouldDig);
        _movementInput.TakeAwayMovementControl();

        _shakes.CustomShake(15, .5f);
        _targetAreaController.TargetGiveHit();
        
        isAttacking = true;
        _allowedToDig = false;
    }

    public void CompleteDigging()
    {
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

    public void DigSound()
    {
        _audio.PlayOneShot(digSounds[Random.Range(0, digSounds.Count)]);
    }
}
