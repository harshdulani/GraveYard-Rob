using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public Image healthBar;
    
    public float waitBeforeRecievingAttackTime = 0.5f;

    private static readonly int ShouldMeleeHash = Animator.StringToHash("shouldMelee");
    private static readonly int DeathHash = Animator.StringToHash("death");
    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private static readonly int HitReceivedHash = Animator.StringToHash("hitReceived");

    private Animator _anim;
    private EnemyStats _enemyStats;
    private Canvas _canvas;
    private Transform _mainCam;
    
    private Quaternion _originalCanvasRotation;

    private PlayerController _playerController;

    private void Awake()
    {
        _canvas = healthBar.GetComponentInParent<Canvas>();
        _canvas.worldCamera = Camera.main;
        _mainCam = Camera.main.transform;
        _originalCanvasRotation = _canvas.transform.rotation;
    }

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
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
        EnemyEvents.current.InvokeEnemyBirth(transform);
        UpdateHealthBar();
    }

    private void LateUpdate()
    {
        //so that health bar canvas always looks at camera in DotaCam
        //_canvas.transform.rotation = _originalCanvasRotation;

        _canvas.transform.rotation = Quaternion.LookRotation(_mainCam.forward, _mainCam.up);
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
        if (other.gameObject.CompareTag("Player"))
        {
            _anim.SetBool(IsMovingHash, false);
            StartCoroutine("OnAttackMelee");            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StopCoroutine("OnAttackMelee");
        }
    }

    public void BiteMaxFront()
    {
        //called by animation event (with reference to this script instead of any object)
        
        if(!PlayerEvents.current.InvokeHealthChange(_enemyStats.meleeDamage))
            PlayerEvents.current.InvokePlayerDeath();
        //if there isn't enough health after a hit, invoke death

    }

    private IEnumerator OnAttackMelee()
    {
        //TODO: remove wait time and add animation events
        //TODO: add bite animation event function 
        yield return new WaitForSeconds(_enemyStats.waitBeforeAttackTime / 1.5f);
        while (true)
        {
            _anim.SetTrigger(ShouldMeleeHash);
            //TODO bump damage/ bump mechanic
            
            /*if(!PlayerEvents.current.InvokeHealthChange(_enemyStats.meleeDamage))
                PlayerEvents.current.InvokePlayerDeath();
            //if there isn't enough health after a hit, invoke death
            */
            
            yield return new WaitForSeconds(_enemyStats.waitBeforeAttackTime);
        }
    }

    private IEnumerator HitReceived()
    {
        _anim.SetTrigger(HitReceivedHash);
        yield return new WaitForSeconds(waitBeforeRecievingAttackTime);
    }

    private void OnPlayerDeath()
    {
        _playerController = null;
        StopAllCoroutines();
    }
}
