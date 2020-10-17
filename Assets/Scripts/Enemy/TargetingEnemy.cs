using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingEnemy : MonoBehaviour
{
    //This is the aim targeting script that belongs on the enemy object
    public Transform target;
    public bool shouldLookAtTarget = false;
    public float waitForTime = 0.1f;

    //orientation calculation
    private Vector3 direction;
    private float angle;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine("TargetingMechanic");
    }

    private IEnumerator TargetingMechanic()
    {
        while (target)
        {
            if (shouldLookAtTarget)
            {
                direction = target.position - transform.position;
                angle = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg);
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }
            yield return new WaitForSeconds(waitForTime);
        }
    }
}