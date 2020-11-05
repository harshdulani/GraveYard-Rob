using Cinemachine;
using System.Collections;
using UnityEngine;

public class TargetingEnemies : MonoBehaviour
{
    //This is the aim targeting script that belongs on the player object
    public Transform target;
    public float waitForTime = 1f;

    private string _targetTag;
    private IEnumerator _targetingMechanic;

    //orientation calculation
    private Vector3 _direction;
    private float _angle;

    //enemy target selection
    public bool DEBUG_ENEMY_FIND_NAMES = false;
    public bool DEBUG_ENEMY_FIND_STATUS = false;
    private float _minDistance, _currentDistance;

    [Header("Cinemachine TargetGroup")] 
    public bool useTargetGroup = false;
    public CinemachineTargetGroup targetCameraHelper;
    public float enemyRadius = 1f, enemyWeight = 1f;

    private void Start()
    {
        _targetTag = "Enemy";
        _targetingMechanic = TargetingMechanic();
        StartCoroutine(_targetingMechanic);
    }
    
    private void OnEnable()
    {
        EnemyEvents.current.enemyDeath += OnEnemyDeath;
    }

    private void OnDisable()
    {
        EnemyEvents.current.enemyDeath -= OnEnemyDeath;
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
        var noOfTargets = GameStats.current.EnemiesAlive;

        if (noOfTargets == 1)
        {
            target = GameStats.current.activeEnemies[0];
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

            //this has been set to an exorbitantly high value so that i dont have to check if this is zero(unset/default) everytime
            _minDistance = 10000f;

            try
            {
                //add a flag so that it doesnt check unless movement is not triggered
                //or no, xerox copy karne ka zarurat naiye archero ka
                foreach (var enemy in GameStats.current.activeEnemies)
                {
                    //this is distance between two points, removed sqrt from formula because increased time complexity and sometimes give NaN
                    var enemyPos = enemy.position;
                    var playerPos = transform.position;
                    _currentDistance = 
                        (enemyPos.x - playerPos.x)* (enemyPos.x - playerPos.x) 
                        + (enemyPos.z - playerPos.z)* (enemyPos.z - playerPos.z);

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

        if (useTargetGroup)
        {
            if (targetCameraHelper.m_Targets.Length == 1)
            {
                targetCameraHelper.AddMember(target, enemyWeight, enemyRadius);
            }
            else if (targetCameraHelper.m_Targets.Length == 2)
            {
                if (!targetCameraHelper.m_Targets[1].target.Equals(target))
                {
                    targetCameraHelper.RemoveMember(targetCameraHelper.m_Targets[1].target);
                    targetCameraHelper.AddMember(target, enemyWeight, enemyRadius);
                }
            }
        }

        return true;
    }

    private void OnEnemyDeath(Transform enemy)
    {
        targetCameraHelper.RemoveMember(enemy);
    }
}
