using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InfernalAttackController : MonoBehaviour
{
    public int damagePerSecond = 150;

    public float slowDownMovementMultiplier = 0.5f;
    public float followPlayerBeforeAttackTime = 2f;

    public GameObject attackIndicator;

    [Header("Audio")]
    public AudioClip tremor;

    private AudioSource _audio;
    
    private bool _isAttacking, _isFollowingPlayer, _waitingToDie, _hasCalledInfernalAttack;
    private float _followTimeRemaining;

    private GameObject _vfx;
    private ParticleSystem _particles;
    private static Transform _player;

    private void Start()
    {
        _vfx = transform.GetChild(0).gameObject;
        _particles = _vfx.transform.GetChild(9).GetComponent<ParticleSystem>();
        _player = PlayerStats.main.transform;

        _audio = GetComponent<AudioSource>();

        GameStats.current.isInfernalAttacking = true;
        //false this in ondisable
        
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
            if (_followTimeRemaining < 0.2f)
            {
                _followTimeRemaining -= Time.deltaTime;
                return;
            }

            _followTimeRemaining -= Time.deltaTime;
            FollowPlayer();
        }
        else
        {
            if(!_hasCalledInfernalAttack)
                InfernalAttack();
        }
    }

    private void StartAttack()
    {
        //Start following Player
        _isAttacking = true;
        _isFollowingPlayer = true;

        _followTimeRemaining = followPlayerBeforeAttackTime;
        _audio.Play();
    }

    private void FollowPlayer()
    {
        if (!_player)
        {
            print("Player not found");
            return;
        }

        var playerPos = _player.position;

        transform.position = new Vector3(playerPos.x, 0.15f, playerPos.z);
    }

    private void InfernalAttack()
    {
        _hasCalledInfernalAttack = true;
        attackIndicator.SetActive(false);
        _vfx.SetActive(true);
        _waitingToDie = true;
        
        
        _audio.Stop();
        _audio.PlayOneShot(tremor);
    }

    private void OnDisable()
    {
        //when attack is being disabled
        GameStats.current.isInfernalAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //start attacking
        //slow movement
        MovementInput.current.SlowDownMovement(slowDownMovementMultiplier);
        
        EnemyEvents.current.InvokeInfernalCautionStart();
    }

    private void OnTriggerStay(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) return;
        
        if(!GameStats.current.isPlayerAlive) return;

        if(!_particles.isPlaying) return;

        //give dps like you heal
        if (!PlayerEvents.current.InvokeHealthChange((damagePerSecond / 50), AttackType.LightAttack))
        {
            PlayerEvents.current.InvokePlayerDeath();
            EnemyEvents.current.InvokeInfernalCautionEnd();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //stop attacking
        //increase movement speed
        MovementInput.current.RestoreMovement();
        EnemyEvents.current.InvokeInfernalCautionEnd();
    }
}