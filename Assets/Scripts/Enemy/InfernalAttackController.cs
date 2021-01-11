using UnityEngine;

public class InfernalAttackController : MonoBehaviour
{
    public int damagePerSecond = 150;

    public float slowDownMovementMultiplier = 0.5f;
    public float followPlayerBeforeAttackTime = 2f;

    public GameObject attackIndicator;
    
    private bool _isAttacking, _isFollowingPlayer, _waitingToDie;
    private float _followTimeRemaining;

    private GameObject _vfx;
    private ParticleSystem _particles;
    private static Transform _player;

    private void Start()
    {
        _vfx = transform.GetChild(0).gameObject;
        _particles = _vfx.transform.GetChild(9).GetComponent<ParticleSystem>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        
        foreach (var system in attackIndicator.GetComponentsInChildren<ParticleSystem>())
        {
            var main = system.main; 
            main.duration = followPlayerBeforeAttackTime;
            system.Play();
        }
        
        StartAttack();
    }

    private void Update()
    {
        if(!_isAttacking) return;
        
        if(_waitingToDie)
            if(_particles.isStopped)
                Destroy(gameObject);
        
        if (_isFollowingPlayer)
        {
            if (_followTimeRemaining < 0f)
            {
                _isFollowingPlayer = false;
                return;
            }

            _followTimeRemaining -= Time.deltaTime;
            FollowPlayer();
        }
        else
        {
            InfernalAttack();
        }
    }

    private void StartAttack()
    {
        //Start following Player
        _isAttacking = true;
        _isFollowingPlayer = true;
        
        _followTimeRemaining = followPlayerBeforeAttackTime;
    }

    private void FollowPlayer()
    {
        if (!_player)
        {
            print("Player not found");
            return;
        }

        transform.position = _player.position;
    }

    private void InfernalAttack()
    {
        attackIndicator.SetActive(false);
        _vfx.SetActive(true);
        _waitingToDie = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //start attacking
        //slow movement
        MovementInput.current.SlowDownMovement(slowDownMovementMultiplier);
    }

    private void OnTriggerStay(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) return;
        
        if(!GameStats.current.isPlayerAlive) return;

        if(!_particles.isPlaying) return;
        
        //give dps like you heal
        if(!PlayerEvents.current.InvokeHealthChange((damagePerSecond / 50), AttackType.LightAttack))
            PlayerEvents.current.InvokePlayerDeath();
    }

    private void OnTriggerExit(Collider other)
    {
        //stop attacking
        //increase movement speed
        MovementInput.current.RestoreMovement();
    }
}