using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public Image healthBar;

    public float waitBeforeAttackTime = 0.5f;
    public float waitBeforeRecievingAttackTime = 0.5f;

    private Animator anim;
    private EnemyStats enemyStats;
    private Quaternion originalHealthBarRotation;

    private void Awake()
    {
        originalHealthBarRotation = healthBar.transform.rotation;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        enemyStats = GetComponent<EnemyStats>();
        UpdateHealthBar();
    }

    private void LateUpdate()
    {
        //so that healthbar always looks at camera
        healthBar.transform.rotation = originalHealthBarRotation;
    }

    public void DecreaseHealth(int amt)
    {
        enemyStats.enemyHealth -= amt;
        UpdateHealthBar();
        if (enemyStats.enemyHealth <= 0)
        {
            //die
            print("Enemy Killed.");
            Debug.Break();
            EnemyDeath();
        }
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)(enemyStats.enemyHealth) / (float)(enemyStats.maxHealth);
    }

    private void EnemyDeath()
    {
        anim.ResetTrigger("hitRecieved");
        anim.ResetTrigger("shouldMelee");
        GetComponent<EnemyFollow>().StopAllCoroutines();
        GetComponent<TargetingEnemy>().StopAllCoroutines();

        anim.SetTrigger("death");
        //reflect in UI
        //consider passing message about enemy death and unsubscribe from enemy
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerWeapon"))
        {
            //DecreaseHealth(collision.gameObject.GetComponent<PlayerWeaponController>().damagePoints);
            StartCoroutine("HitRecieved");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            anim.SetBool("isMoving", false);
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
        while (true)
        {
            yield return new WaitForSeconds(waitBeforeAttackTime);
            anim.SetTrigger("shouldMelee"); 
            FindObjectOfType<PlayerController>().DecreaseHealth(enemyStats.bumpDamage);            
        }
    }

    private IEnumerator HitRecieved()
    {
        anim.SetTrigger("hitRecieved");
        yield return new WaitForSeconds(waitBeforeRecievingAttackTime);
    }
}
