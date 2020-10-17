using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int bumpDamage = 50;

    public float waitBeforeAttackTime = 0.5f;
    public float waitBeforeRecievingAttackTime = 0.5f;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerWeapon"))
        {
            StartCoroutine("HitRecieved");
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerWeapon"))
        {
            StopCoroutine("HitRecieved");
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
            FindObjectOfType<PlayerController>().DecreaseHealth(bumpDamage);            
        }
    }

    private IEnumerator HitRecieved()
    {
        while (true)
        {
            anim.SetTrigger("hitRecieved");
            yield return new WaitForSeconds(waitBeforeRecievingAttackTime);
        }
    }
}
