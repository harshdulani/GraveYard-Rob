using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public int damagePoints = 100;

    public float waitBeforeAttackTime = 0.75f;

    private bool canGiveHit = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (canGiveHit)
            {
                Vector3 playerForward = FindObjectOfType<MovementInput>().transform.TransformDirection(Vector3.forward);
                Vector3 toOther = collision.transform.position - FindObjectOfType<MovementInput>().transform.position;
                
                if (Vector3.Dot(playerForward.normalized, toOther.normalized) > 0)
                {
                    StartCoroutine(GiveHit(collision.gameObject.GetComponent<EnemyController>()));
                }
            }
        }
    }

    private IEnumerator GiveHit(EnemyController enemyController)
    {
        enemyController.DecreaseHealth(damagePoints);
        canGiveHit = false;
        yield return new WaitForSeconds(waitBeforeAttackTime);
        canGiveHit = true;
    }
}