using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    const float PathUpdateMoveThreshold = 0.5f;

    public float minPathUpdateTime = 0.2f;

    private Transform _target;
    private NavMeshAgent _agent;
    private Vector3 _targetPosOld;
    private Animator _anim;
    private TargetingEnemy _targetingEnemy;
    
    
    private static readonly int IsMoving = Animator.StringToHash("isMoving");

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _targetingEnemy = GetComponent<TargetingEnemy>();

        _target = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(FollowMechanic());
    }

    private IEnumerator FollowMechanic()
    {
        while (true)
        {            
            yield return new WaitForSeconds(minPathUpdateTime);
            if (_target && !_agent.isStopped)
            {
                if ((_target.position - _targetPosOld).sqrMagnitude > (PathUpdateMoveThreshold * PathUpdateMoveThreshold))
                {
                    _agent.SetDestination(_target.position);
                    _anim.SetBool(IsMoving, true);
                }
            }
            _targetPosOld = _target.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            _agent.isStopped = true;
            _anim.SetBool(IsMoving, false);
            
            _targetingEnemy.shouldLookAtTarget = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _agent.isStopped = false;
            _anim.SetBool(IsMoving, true);
            
            _targetingEnemy.shouldLookAtTarget = false;
        }
    }
}