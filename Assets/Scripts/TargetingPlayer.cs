using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingPlayer : MonoBehaviour
{
    //This is the aim targeting script that belongs on the player object
    public Transform target;
    public float waitForTime = 1f;

    private string targetTag;

    //orientation calculation
    private Vector3 direction;
    private float angle;

    //enemy target selection
    public bool DEBUG_ENEMY_FIND = false;
    private GameObject[] enemyTargets;
    private float minDistance, currentDistance;

    private void Start()
    {
        targetTag = "Enemy";
        StartCoroutine("TargetingMechanic");
    }

    private IEnumerator TargetingMechanic()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitForTime);
            if(!PlayerMovement.isMoving)
                if (!FindTarget())
                    print("Find Target failure for player.");
        }
    }

    private bool FindTarget()
    {
        //make this part of a GameController singleton, so that there is a count on the current enemies available,
        //so I don't have to look up the number all the times
        var noOfTargets = GameObject.FindGameObjectsWithTag(targetTag).Length;

        if (noOfTargets == 1)
        {
            target = GameObject.FindGameObjectWithTag(targetTag).transform;
            print("found 1 target named " + target.name);
        }
        else if (noOfTargets == 0)
        {
            print("No targets found for " + gameObject.name);
            return false;
        }
        else if (noOfTargets > 1)
        {
            //Targeting Multiple Enemies
            enemyTargets = GameObject.FindGameObjectsWithTag(targetTag);

            //this has been set to an exorbitantly high value so that i dont have to check if this is zero(unset/default) everytime
            minDistance = 10000f;

            try
            {
                //add a flag so that it doesnt check unless the position is changed
                //or no, xerox copy karne ka zarurat naiye
                foreach (var enemy in enemyTargets)
                {
                    //this is distance between two points, removed sqrt from formula because increased time complex and sometimes give NaN
                    currentDistance = 
                        (enemy.transform.position.x - transform.position.x)* (enemy.transform.position.x - transform.position.x) 
                        + (enemy.transform.position.z - transform.position.z)* (enemy.transform.position.z - transform.position.z);

                    if (DEBUG_ENEMY_FIND)
                        print("current enemy = " + enemy.name + ", distance = " + currentDistance);
                    if (minDistance > currentDistance)
                    {
                        minDistance = currentDistance;
                        target = enemy.transform;
                    }
                }
                if (DEBUG_ENEMY_FIND)
                    print("target = " + target.gameObject.name);
            }
            catch
            {
                print("Problem occurred with finding enemy targets.");
            }
        }

        direction = target.position - transform.position;
        angle = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

        return true;
    }
}
