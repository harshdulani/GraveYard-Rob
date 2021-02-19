using System;
using UnityEngine;

public class EnemyEvents : MonoBehaviour
{
    public static EnemyEvents current;

    public Action<Transform> enemyBirth, enemyDeath;
    public Action infernalAttackCautionStart, infernalAttackCautionEnd;
    
    private void Awake()
    {
        if (current == null)
            current = this;
        else
            Destroy(this);
    }

    public void InvokeEnemyBirth(Transform enemy)
    {
        enemyBirth?.Invoke(enemy);
    }

    public void InvokeEnemyDeath(Transform enemy)
    {
        enemyDeath?.Invoke(enemy);
    }

    public void InvokeInfernalCautionStart()
    {
        infernalAttackCautionStart?.Invoke();
    }
    
    public void InvokeInfernalCautionEnd()
    {
        infernalAttackCautionEnd?.Invoke();
    }
}
