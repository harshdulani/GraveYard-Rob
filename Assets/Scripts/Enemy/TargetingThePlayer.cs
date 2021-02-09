using System.Collections;
using UnityEngine;

public class TargetingThePlayer : MonoBehaviour
{
    //This is the aim targeting script that belongs on the enemy object
    public Transform target;
    public bool shouldLookAtTarget = false;
    public float waitForTime = 0.1f;

    //orientation calculation
    private Vector3 _direction;
    private float _angle;
    private IEnumerator _targetingMech;

    private WaitForSeconds _waitTargeting;
    
    private void OnEnable()
    {
        EnemyEvents.current.enemyDeath += OnEnemyDeath;
        
        PlayerEvents.current.playerDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        EnemyEvents.current.enemyDeath -= OnEnemyDeath;
        
        PlayerEvents.current.playerDeath -= OnPlayerDeath;
    }

    private void Start()
    {
        target = PlayerStats.main.transform;
        _targetingMech = TargetingMechanic();
        
        _waitTargeting = new WaitForSeconds(waitForTime);
        
        StartCoroutine(_targetingMech);
    }

    private IEnumerator TargetingMechanic()
    {
        while (target)
        {
            if (shouldLookAtTarget)
            {
                _direction = target.position - transform.position;
                _angle = (Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg);
                transform.rotation = Quaternion.AngleAxis(_angle, Vector3.up);
            }
            yield return _waitTargeting;
        }
    }

    private void OnPlayerDeath()
    {
        StopAllCoroutines();
    }

    private void OnEnemyDeath(Transform enemy)
    {
        if (enemy == transform)
        {
            StopCoroutine(_targetingMech);
            enabled = false;
        }
    }
}