using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour
{
    public Transform target;
    public float waitForTime = 1f;

    private string targetTag;

    //target calculation
    private Vector3 direction;
    private float angle;

    private void Start()
    {
        if (gameObject.CompareTag("Player"))
            targetTag = "Enemy";
        else
        {
            targetTag = "Player";
        }
        StartCoroutine("TargetingMechanic");
    }

    private IEnumerator TargetingMechanic()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitForTime);
            if (!FindTarget())
                print("Find Target failure.");
        }
    }

    private bool FindTarget()
    {
        target = GameObject.FindGameObjectWithTag(targetTag).transform;

        var noOfTargets = GameObject.FindGameObjectsWithTag(targetTag).Length;

        if (noOfTargets == 0)
        {
            print("No targets found for " + gameObject.name);
            return false;
        }
        else if (noOfTargets > 1)
        {
            print("Write enemy targeting code here");
            return false;
        }
        else
        {
            print("Targeting Player now");
        }

        direction = target.position - transform.position;
        angle = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

        return true;
    }
}