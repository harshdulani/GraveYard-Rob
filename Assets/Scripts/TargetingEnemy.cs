using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingEnemy : MonoBehaviour
{
    //This is targeting script that belongs on the enemy object
    public Transform target;
    public float waitForTime = 1f;

    private string targetTag;

    //orientation calculation
    private Vector3 direction;
    private float angle;   

    private void Start()
    {
        targetTag = "Player";

        StartCoroutine("TargetingMechanic");
    }

    private IEnumerator TargetingMechanic()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitForTime);
            if (!FindTarget())
                print("Find Target failure for Enemy " + gameObject.name);
        }
    }

    private bool FindTarget()
    {
        if(GameObject.FindGameObjectWithTag(targetTag))
            target = GameObject.FindGameObjectWithTag(targetTag).transform;
        else
        {
            print(gameObject.name + " didn't find player.");
            return false;
        }

        direction = target.position - transform.position;
        angle = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

        return true;
    }
}