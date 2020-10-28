using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public int damagePoints = 100;
    
    public bool shouldGiveHit = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (shouldGiveHit)
            {
                Vector3 playerForward = FindObjectOfType<MovementInput>().transform.TransformDirection(Vector3.forward);
                Vector3 toOther = collision.transform.position - FindObjectOfType<MovementInput>().transform.position;
                
                if (Vector3.Dot(playerForward.normalized, toOther.normalized) > 0)
                {
                    //making this false because hit already given
                    shouldGiveHit = false;
                    //add wait time before they can get hit to enemy & player scripts 
                    collision.gameObject.GetComponent<EnemyController>().DecreaseHealth(damagePoints);
                }
            }
        }
    }
}