using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public Image healthBar;
    
    public float waitBeforeReceivingAttackTime = 0.5f;
    public float timeBeforeInflictingBumpDamage = 2f;

    private bool _isPlayerInContact;
    private bool _canBump = true;
    private float _myBumpDamageTimer;


    private static readonly int ShouldMeleeHash = Animator.StringToHash("shouldMelee");
    private static readonly int DeathHash = Animator.StringToHash("death");
    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private static readonly int HitReceivedHash = Animator.StringToHash("hitReceived");

    private Animator _anim;
    private EnemyStats _enemyStats;

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
        
        EnemyEvents.current.InvokeEnemyBirth(transform);
        UpdateHealthBar();
    }

    private void Update()
    {
        if(_canBump) return;
        if (!(_myBumpDamageTimer > 0)) return;
        
        _myBumpDamageTimer -= Time.deltaTime;

        if (_myBumpDamageTimer <= 0)
            //time's up
            _canBump = true;
    }

    public void DecreaseHealth(int amt)
    {
        if (_enemyStats.TakeHit(amt))
        {
            //die
            print("Enemy Killed.");
            EnemyEvents.current.InvokeEnemyDeath(transform);
            EnemyDeath();
        }
        else
            StartCoroutine(nameof(HitReceived));
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)(_enemyStats.enemyHealth) / (float)(_enemyStats.maxHealth);
    }

    private void EnemyDeath()
    {
        //so that no more collisions are detected
        GetComponent<CapsuleCollider>().enabled = false;

        StopAllCoroutines();
        _anim.ResetTrigger(ShouldMeleeHash);

        _anim.SetTrigger(DeathHash);

        //reflect in UI
        Destroy(gameObject, 1.75f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        _anim.SetBool(IsMovingHash, false);
        _isPlayerInContact = true;
        StartCoroutine("OnAttackMelee");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        _isPlayerInContact = false;
        StopCoroutine("OnAttackMelee");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        //if we give bump damage when they're attacking then there is health loss at every player attack too
        if(other.gameObject.GetComponent<PlayerCombat>().isAttacking) return;
        
        //bump damage
        if(!PlayerEvents.current.InvokeHealthChange(_enemyStats.bumpDamage))
            PlayerEvents.current.InvokePlayerDeath();
        //if there isn't enough health after a hit, invoke death
            
            
        //reset timer
        _myBumpDamageTimer = timeBeforeInflictingBumpDamage;
    }

    public void BiteMaxFront()
    {
        //called by animation event (with reference to this script instead of any object)
        
        if(!_isPlayerInContact) return;
        
        //now check whether the player is still in front of the enemy when it delivers its attack,
        //to see if an attack should actually be delivered
        Vector3 enemyForward = transform.TransformDirection(Vector3.forward);
        Vector3 toPlayer = MovementInput.current.transform.position - transform.position;
                
        //if
        if (Vector3.Dot(enemyForward.normalized, toPlayer.normalized) <= 0)
            return;
        
        if(!PlayerEvents.current.InvokeHealthChange(_enemyStats.meleeDamage))
            PlayerEvents.current.InvokePlayerDeath();
        //if there isn't enough health after a hit, invoke death
    }

    private IEnumerator OnAttackMelee()
    {
        yield return new WaitForSeconds(_enemyStats.waitBeforeAttackTime / 1.5f);
        while (true)
        {
            //attack animation begins here and then when bite is at maximum front, attack is placed
            _anim.SetTrigger(ShouldMeleeHash);
            
            yield return new WaitForSeconds(_enemyStats.waitBeforeAttackTime);
        }
    }

    private IEnumerator HitReceived()
    {
        _anim.SetTrigger(HitReceivedHash);
        yield return new WaitForSeconds(waitBeforeReceivingAttackTime);
    }

    private void OnPlayerDeath()
    {
        StopAllCoroutines();
    }
}
