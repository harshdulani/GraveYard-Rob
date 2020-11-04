using System;
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
    private Vector3 _direction;
    private float _angle;
    private IEnumerator _targetingMech;

    private void OnEnable()
    {
        EnemyEvents.current.enemyDeath += OnEnemyDeath;
    }

    private void OnDisable()
    {
        EnemyEvents.current.enemyDeath -= OnEnemyDeath;
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        target.GetComponent<PlayerController>().PlayerDeath += PlayerHasDied;

        _targetingMech = TargetingMechanic();
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
            yield return new WaitForSeconds(waitForTime);
        }
    }

    private void PlayerHasDied()
    {
        target.GetComponent<PlayerController>().PlayerDeath -= PlayerHasDied;
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