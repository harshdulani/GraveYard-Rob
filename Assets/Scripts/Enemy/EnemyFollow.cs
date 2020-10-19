using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    const float pathUpdateMoveThreshold = 0.5f;

    public float minPathUpdateTime = 0.2f;

    private Transform target;
    private NavMeshAgent agent;
    private Vector3 targetPosOld;
    private Animator anim;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        target = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine("FollowMechanic");
    }

    private IEnumerator FollowMechanic()
    {
        while (true)
        {            
            yield return new WaitForSeconds(minPathUpdateTime);
            if (target && !agent.isStopped)
            {
                if ((target.position - targetPosOld).sqrMagnitude > (pathUpdateMoveThreshold * pathUpdateMoveThreshold))
                {
                    agent.SetDestination(target.position);
                    GetComponent<Animator>().SetBool("isMoving", true);
                }
            }
            targetPosOld = target.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            agent.isStopped = true;
            anim.SetBool("isMoving", false);
            GetComponent<TargetingEnemy>().shouldLookAtTarget = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            agent.isStopped = false;
            anim.SetBool("isMoving", true);
            GetComponent<TargetingEnemy>().shouldLookAtTarget = false;
        }
    }
}