using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public Image healthBar;

    public float waitBeforeAttackTime = 0.5f;
    public float waitBeforeRecievingAttackTime = 0.5f;

    private static int shouldMeleeHash, deathHash, isMovingHash, hitRecievedHash;

    private Animator anim;
    private EnemyStats enemyStats;
    private Quaternion originalCanvasRotation;
    private Canvas canvas;

    private void Awake()
    {
        canvas = healthBar.GetComponentInParent<Canvas>();
        canvas.worldCamera = Camera.main;
        originalCanvasRotation = canvas.transform.rotation;
    }

    private void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        anim = GetComponent<Animator>();
        UpdateHealthBar();

        if (shouldMeleeHash == 0)
        {
            //if one is zero all must be, check here if you have changed a name of a parameter
            shouldMeleeHash = Animator.StringToHash("shouldMelee");
            deathHash = Animator.StringToHash("death");
            isMovingHash = Animator.StringToHash("isMoving");
            hitRecievedHash = Animator.StringToHash("hitRecieved");
        }
    }

    private void LateUpdate()
    {
        //so that healthbar canvas always looks at camera
        canvas.transform.rotation = originalCanvasRotation;
    }

    public void DecreaseHealth(int amt)
    {
        enemyStats.enemyHealth -= amt;
        UpdateHealthBar();
        if (enemyStats.enemyHealth <= 0)
        {
            //die
            print("Enemy Killed.");
            EnemyDeath();
        }
        else
            StartCoroutine("HitRecieved");
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)(enemyStats.enemyHealth) / (float)(enemyStats.maxHealth);
    }

    private void EnemyDeath()
    {
        anim.ResetTrigger(shouldMeleeHash);

        GetComponent<EnemyFollow>().StopAllCoroutines();
        GetComponent<TargetingEnemy>().StopAllCoroutines();

        //destroying and not just disabling this because enabling it didnt stop it from following player
        Destroy(GetComponent<EnemyFollow>());
        GetComponent<TargetingEnemy>().enabled = false;

        //so that no more collisions are detected
        GetComponent<CapsuleCollider>().enabled = false;

        StopAllCoroutines();

        anim.SetTrigger(deathHash);

        //reflect in UI
        //consider passing message about enemy death and unsubscribe from enemy
        Destroy(gameObject, 1.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            anim.SetBool(isMovingHash, false);
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
            anim.SetTrigger(shouldMeleeHash); 
            FindObjectOfType<PlayerController>().DecreaseHealth(enemyStats.bumpDamage);
            yield return new WaitForSeconds(waitBeforeAttackTime);
        }
    }

    private IEnumerator HitRecieved()
    {
        anim.SetTrigger(hitRecievedHash);
        yield return new WaitForSeconds(waitBeforeRecievingAttackTime);
    }
}
