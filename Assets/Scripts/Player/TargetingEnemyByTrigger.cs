using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetingEnemyByTrigger : MonoBehaviour
{
    public List<Transform> enemies;

    private void OnEnable()
    {
        PlayerEvents.current.startCombatStrike += OnCombatStrikeStart;
    }
    
    
    private void OnDisable()
    {
        PlayerEvents.current.startCombatStrike -= OnCombatStrikeStart;
    }


    private void Start()
    {
        enemies = new List<Transform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Enemy")) return;
        
        //there are 2 colliders - 1 capsule and 1 trigger on each enemy, hence contains
        if (!enemies.Contains(other.transform))
        {
            //print("adding same for some reason, maybe destroy doesn't call ontriggerexit");
            print(other.gameObject.name + " entered");
            enemies.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.gameObject.CompareTag("Enemy")) return;

        if (enemies.Contains(other.transform))
        {
            enemies.Remove(other.transform);
            print(other.gameObject.name + " exit");
        }
    }

    private void OnCombatStrikeStart()
    {
        FindTarget();
    }

    private void FindTarget()
    {
        
    }
}
