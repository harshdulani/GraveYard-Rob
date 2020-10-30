using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public Image healthBar;

    public float waitBeforeAttackTime = 0.5f;
    public float waitBeforeRecievingAttackTime = 0.5f;
    
    private static readonly int ShouldMeleeHash = Animator.StringToHash("shouldMelee");
    private static readonly int DeathHash = Animator.StringToHash("death");
    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private static readonly int HitReceivedHash = Animator.StringToHash("hitReceived");

    private Animator _anim;
    private EnemyStats _enemyStats;
    private EnemyFollow _enemyFollow;
    private TargetingEnemy _targetingEnemy;
    
    private Quaternion _originalCanvasRotation;
    private Canvas _canvas;

    private PlayerController _playerController;

    private void Awake()
    {
        _canvas = healthBar.GetComponentInParent<Canvas>();
        _canvas.worldCamera = Camera.main;
        _originalCanvasRotation = _canvas.transform.rotation;
    }

    private void Start()
    {
        _enemyStats = GetComponent<EnemyStats>();
        _anim = GetComponent<Animator>();
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _enemyFollow = GetComponent<EnemyFollow>();
        _targetingEnemy = GetComponent<TargetingEnemy>();
        
        _playerController.playerDeath += PlayerHasDied;

        UpdateHealthBar();
    }

    private void LateUpdate()
    {
        //so that healthbar canvas always looks at camera
        _canvas.transform.rotation = _originalCanvasRotation;
    }

    public void DecreaseHealth(int amt)
    {
        _enemyStats.enemyHealth -= amt;
        UpdateHealthBar();
        if (_enemyStats.enemyHealth <= 0)
        {
            //die
            print("Enemy Killed.");
            EnemyDeath();
        }
        else
            StartCoroutine(nameof(HitRecieved));
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)(_enemyStats.enemyHealth) / (float)(_enemyStats.maxHealth);
    }

    private void EnemyDeath()
    {
        _enemyFollow.StopAllCoroutines();
        _targetingEnemy.StopAllCoroutines();

        //destroying and not just disabling this because enabling it didnt stop it from following player
        Destroy(_enemyFollow);
        _targetingEnemy.enabled = false;

        //so that no more collisions are detected
        GetComponent<CapsuleCollider>().enabled = false;

        StopAllCoroutines();
        _anim.ResetTrigger(ShouldMeleeHash);

        _anim.SetTrigger(DeathHash);

        //reflect in UI
        //consider passing message about enemy death and unsubscribe from enemy
        Destroy(gameObject, 1.75f);
    }

    private void PlayerHasDied()
    {
        _playerController.playerDeath -= PlayerHasDied;
        _enemyFollow.StopAllCoroutines();
        _targetingEnemy.StopAllCoroutines();
    }

    private void OnDestroy()
    {
        _playerController.playerDeath -= PlayerHasDied;
        _enemyFollow.StopAllCoroutines();
        _targetingEnemy.StopAllCoroutines();
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

    private IEnumerator OnAttackMelee()
    {
        yield return new WaitForSeconds(waitBeforeAttackTime / 1.5f);
        while (true)
        {
            _anim.SetTrigger(ShouldMeleeHash); 
            FindObjectOfType<PlayerController>().DecreaseHealth(_enemyStats.bumpDamage);
            yield return new WaitForSeconds(waitBeforeAttackTime);
        }
    }

    private IEnumerator HitRecieved()
    {
        _anim.SetTrigger(HitReceivedHash);
        yield return new WaitForSeconds(waitBeforeRecievingAttackTime);
    }
}
