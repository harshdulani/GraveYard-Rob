using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingPlayer : MonoBehaviour
{
    //This is the aim targeting script that belongs on the player object
    public Transform target;
    public float waitForTime = 1f;

    private string _targetTag;

    //orientation calculation
    private Vector3 _direction;
    private float _angle;

    //enemy target selection
    public bool DEBUG_ENEMY_FIND_NAMES = false;
    public bool DEBUG_ENEMY_FIND_STATUS = false;
    private GameObject[] _enemyTargets;
    private float _minDistance, _currentDistance;

    //cinemachine
    public CinemachineTargetGroup targetCameraHelper;
    public float enemyRadius = 1f, enemyWeight = 1f;

    private void Start()
    {
        _targetTag = "Enemy";
        StartCoroutine("TargetingMechanic");
    }

    private IEnumerator TargetingMechanic()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitForTime);
            if(!PlayerMovement.IsMoving)
                if (!FindTarget())
                    if(DEBUG_ENEMY_FIND_STATUS)
                        print("Find Target failure for player.");
        }
    }

    private bool FindTarget()
    {
        //make this part of a GameController singleton, so that there is a count on the current enemies available,
        //so I don't have to look up the number all the times
        var noOfTargets = GameObject.FindGameObjectsWithTag(_targetTag).Length;

        if (noOfTargets == 1)
        {
            target = GameObject.FindGameObjectWithTag(_targetTag).transform;
            if (DEBUG_ENEMY_FIND_STATUS)
                print("found 1 target named " + target.name);
        }
        else if (noOfTargets == 0)
        {
            target = null; 
            if (DEBUG_ENEMY_FIND_STATUS)
                print("No targets found for " + gameObject.name);
            return false;
        }
        else if (noOfTargets > 1)
        {
            //Targeting Multiple Enemies
            _enemyTargets = GameObject.FindGameObjectsWithTag(_targetTag);

            //this has been set to an exorbitantly high value so that i dont have to check if this is zero(unset/default) everytime
            _minDistance = 10000f;

            try
            {
                //add a flag so that it doesnt check unless movement is not triggered
                //or no, xerox copy karne ka zarurat naiye archero ka
                foreach (var enemy in _enemyTargets)
                {
                    //this is distance between two points, removed sqrt from formula because increased time complex and sometimes give NaN
                    _currentDistance = 
                        (enemy.transform.position.x - transform.position.x)* (enemy.transform.position.x - transform.position.x) 
                        + (enemy.transform.position.z - transform.position.z)* (enemy.transform.position.z - transform.position.z);

                    if (DEBUG_ENEMY_FIND_NAMES)
                        print("current enemy = " + enemy.name + ", distance = " + _currentDistance);
                    if (_minDistance > _currentDistance)
                    {
                        _minDistance = _currentDistance;
                        target = enemy.transform;
                    }
                }
                if (DEBUG_ENEMY_FIND_NAMES)
                    print("target = " + target.gameObject.name);
            }
            catch
            {
                print("Problem occurred with finding enemy targets.");
            }
        }

        if(targetCameraHelper.m_Targets.Length == 1)
        {
            targetCameraHelper.AddMember(target, enemyWeight, enemyRadius);
        }
        else if(targetCameraHelper.m_Targets.Length == 2)
        {
            if (!targetCameraHelper.m_Targets[1].target.Equals(target))
            {
                targetCameraHelper.RemoveMember(targetCameraHelper.m_Targets[1].target);
                targetCameraHelper.AddMember(target, enemyWeight, enemyRadius);
            }
        }

        return true;
    }

    // filhaal keep this as a notification of the enemy BY THE ENEMY that is called at enemy birth
    // later change it to gamecontroller notifying
    public void BirthNotify(EnemyShooting enemy)
    {
        enemy.destructionEvent += OnEnemyDeath;
        print("subscribed to " + enemy.gameObject.name);
    }

    private void OnEnemyDeath(Transform enemyTransform)
    {
        print(enemyTransform.gameObject.name + " just died");
        targetCameraHelper.RemoveMember(enemyTransform);
        enemyTransform.GetComponent<EnemyShooting>().destructionEvent -= OnEnemyDeath;
        print("unsubscribed to " + enemyTransform.gameObject.name);
    }
}
