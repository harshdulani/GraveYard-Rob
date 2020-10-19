using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public int damagePoints = 100;

    public float waitBeforeAttackTime = 0.75f;

    public bool isGivingHit = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (isGivingHit)
            {
                Vector3 playerForward = FindObjectOfType<MovementInput>().transform.TransformDirection(Vector3.forward);
                Vector3 toOther = collision.transform.position - FindObjectOfType<MovementInput>().transform.position;
                
                if (Vector3.Dot(playerForward.normalized, toOther.normalized) > 0)
                {
                    collision.gameObject.GetComponent<EnemyController>().DecreaseHealth(damagePoints);
                }
            }
        }
    }
}