using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public Image healthBar;

    public GameObject deathVFX;
    public List<AudioClip> hitSFX;
    public AudioClip bite;

    public float timeBeforeInflictingBumpDamage = 2f;
    
    [Header("Movement Speed")]
    public float minimumEnemyMovementSpeed = 3f;
    public float enemyMovementSpeedVariability = 8f;
    
    private bool _isPlayerInContact;
    private bool _canBump = true;
    private float _myBumpDamageTimer;
    
    private Animator _anim;
    private EnemyStats _enemyStats;
    private ScreenShakes _shakes;
    private Rigidbody _rigidbody;
    private AudioSource _audio;

    [Header("Hit FreezeFrame / HitStop")] 
    public float deathFreezeFrame = 0.2f;
    public float heavyAttackFreezeFrame = 0.075f;

    private WaitForSeconds _waitBeforeAttack, _waitBeforeFirstAttack;
    private WaitForSecondsRealtime _death, _heavyAttack;

    private static readonly int ShouldMeleeHash = Animator.StringToHash("shouldMelee");
    private static readonly int DeathHash = Animator.StringToHash("death");
    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private static readonly int HitReceivedHash = Animator.StringToHash("hitReceived");
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
        _enemyStats = GetComponent<EnemyStats>();
        _anim = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _shakes = GetComponent<ScreenShakes>();
        _audio = GetComponent<AudioSource>();

        if (_enemyStats.type == EnemyType.Ghost)
        {
            GetComponent<NavMeshAgent>().speed = Random.Range(minimumEnemyMovementSpeed,
                minimumEnemyMovementSpeed + enemyMovementSpeedVariability);
        }
        
        EnemyEvents.current.InvokeEnemyBirth(transform);
        UpdateHealthBar();

        _waitBeforeAttack = new WaitForSeconds(_enemyStats.waitBeforeAttackTime);
        _waitBeforeFirstAttack = new WaitForSeconds(_enemyStats.waitBeforeAttackTime / 1.5f);
        
        
        _heavyAttack = new WaitForSecondsRealtime(heavyAttackFreezeFrame);
        _death = new WaitForSecondsRealtime(deathFreezeFrame);
    }

    private void Update()
    {
        if(_canBump) return;
        if (!(_myBumpDamageTimer > 0)) return;
        
        if(GameStats.current.isPlayerAlive && _rigidbody.IsSleeping()) _rigidbody.WakeUp();
        
        _myBumpDamageTimer -= Time.deltaTime;

        if (_myBumpDamageTimer <= 0)
            //time's up
            _canBump = true;
    }

    public void DecreaseHealth(int amt)
    {
        if(amt == PlayerStats.main.lightAttackDamage)
            _shakes.Light();
        else if (amt == PlayerStats.main.heavyAttackDamage)
        {
            _shakes.Heavy();
            StartCoroutine(FreezeFrame(_heavyAttack));
        }
        
        //the impact sound
        _audio.PlayOneShot(hitSFX[2], 0.8f);
        //pain sound
        _audio.PlayOneShot(hitSFX[Random.Range(0, 2)], 0.5f);
        
        if (_enemyStats.TakeHit(amt))
        {
            //die
            print("Enemy Killed.");
            StopAllCoroutines();
            StartCoroutine(FreezeFrame(_death));
            EnemyEvents.current.InvokeEnemyDeath(transform);
            //EnemyDeath();
        }
        else
        {
            _anim.SetTrigger(HitReceivedHash);
        }
        UpdateHealthBar();
    }
    
    private IEnumerator FreezeFrame(WaitForSecondsRealtime wait)
    {
        Time.timeScale = 0f;
        yield return wait;
        Time.timeScale = 1f;
        if(wait == _death)
            EnemyDeath();
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)(_enemyStats.enemyHealth) / (float)(_enemyStats.maxHealth);
    }

    private void EnemyDeath()
    {
        //so that no more collisions are detected
        GetComponent<CapsuleCollider>().enabled = false;
        //so that non NavMesh Agents don't fall through the ground
        GetComponent<Rigidbody>().useGravity = false;

        StopAllCoroutines();

        switch (_enemyStats.type)
        {
            case EnemyType.Ghost:
                _anim.ResetTrigger(ShouldMeleeHash);
                break;
            case EnemyType.Demon:
                _anim.ResetTrigger(ShouldRanged);
                break;
        }

        _anim.SetTrigger(DeathHash);
        _audio.PlayOneShot(hitSFX[3], 0.8f);
        _audio.PlayOneShot(hitSFX[4], 0.8f);

        Instantiate(deathVFX, transform.position + Vector3.up * 2f, Quaternion.identity, gameObject.transform);
        Destroy(transform.GetChild(0).gameObject, 2.5f);
        Destroy(gameObject, 3.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        if(_enemyStats.type != EnemyType.Ghost) return;
        
        _anim.SetBool(IsMovingHash, false);
        _isPlayerInContact = true;
        
        StartCoroutine(OnAttackMelee());
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        if(_enemyStats.type != EnemyType.Ghost) return;
        
        _isPlayerInContact = false;
        StopCoroutine("OnAttackMelee");
        _anim.ResetTrigger(ShouldMeleeHash);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        //if we give bump damage when they're attacking then there is health loss at every player attack too
        if(other.gameObject.GetComponent<PlayerCombat>().isAttacking) return;
        
        //bump damage
        if(!PlayerEvents.current.InvokeHealthChange(_enemyStats.bumpDamage, AttackType.LightAttack))
            PlayerEvents.current.InvokePlayerDeath();
        //if there isn't enough health after a hit, invoke death

        //reset timer
        _myBumpDamageTimer = timeBeforeInflictingBumpDamage;
    }

    public void BiteMaxFront()
    {
        if (!GameStats.current.isPlayerAlive) return;
        if(!GameStats.current.isGamePlaying) return;
        
        //called by animation event (with reference to this script instead of any object)
        if(!_isPlayerInContact) return;

        //now check whether the player is still in front of the enemy when it delivers its attack,
        //to see if an attack should actually be delivered
        Vector3 enemyForward = transform.TransformDirection(Vector3.forward);
        Vector3 toPlayer = MovementInput.current.transform.position - transform.position;
                
        //if
        if (Vector3.Dot(enemyForward.normalized, toPlayer.normalized) <= 0)
            return;
        
        if (!GameStats.current.isPlayerAlive) return;
        if(!GameStats.current.isGamePlaying) return;
        
        if(!PlayerEvents.current.InvokeHealthChange(_enemyStats.attackDamage, AttackType.HeavyAttack))
            PlayerEvents.current.InvokePlayerDeath();
        //if there isn't enough health after a hit, invoke death
        
        _audio.PlayOneShot(bite, 0.6f);
    }

    private IEnumerator OnAttackMelee()
    {
        yield return _waitBeforeFirstAttack;
        while (true)
        {
            if (!GameStats.current.isPlayerAlive) yield break;
            
            //attack animation begins here and then when bite is at maximum front, attack is placed
            _anim.SetTrigger(ShouldMeleeHash);

            yield return _waitBeforeAttack;
        }
    }

    private void OnPlayerDeath()
    {
        StopCoroutine("OnAttackMelee");
        StopAllCoroutines();
    }
}