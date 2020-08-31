using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour
{
    public Transform target;
    public float waitForTime = 1f;

    private string targetTag;    

    //orientation calculation
    private Vector3 direction;
    private float angle;

    //enemy target selection
    private GameObject[] enemyTargets;
    private float minDistance, currentDistance;

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
            enemyTargets = GameObject.FindGameObjectsWithTag(targetTag);

            minDistance = 0f;

            try
            {
                //add a flag so that it doesnt check unless the position is changed
                foreach (var enemy in enemyTargets)
                {
                    //try if it works the same without the sqrt
                    currentDistance = (enemy.transform.position.x - transform.position.x) - (enemy.transform.position.z - transform.position.z);

                    print("current enemy = " + enemy.name + ", distance = " + currentDistance);

                    //shouldnt make this check everytime, if min dist will be 0 only once, better try keeping it to an exorbitantly high value
                    if (minDistance == 0f || minDistance > Mathf.Abs(currentDistance))
                    {
                        minDistance = Mathf.Abs(currentDistance);
                        target = enemy.transform;
                    }
                }
                print("target = " + target.gameObject.name);
            }
            catch
            {
                print("Problem occurred with finding enemy targets.");
            }
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