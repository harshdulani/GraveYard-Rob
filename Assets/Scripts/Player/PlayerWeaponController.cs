using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public bool shouldGiveHit;

    private List<GameObject> _enemiesAttacked = new List<GameObject>(); 

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Enemy")) return;

        if (!shouldGiveHit) return;
        //this value is also changed by PlayerCombat
        
        Vector3 playerForward = MovementInput.current.transform.TransformDirection(Vector3.forward);
        Vector3 toOther = other.transform.position - MovementInput.current.transform.position;
                
        if (Vector3.Dot(playerForward.normalized, toOther.normalized) > 0)
        {
            if (PlayerCombat.currentAttackType == AttackType.LightAttack)
            {
                shouldGiveHit = false;
                other.gameObject.GetComponent<EnemyController>().DecreaseHealth(PlayerStats.main.lightAttackDamage);
            }
            else if (PlayerCombat.currentAttackType == AttackType.HeavyAttack)
            {
                if (!_enemiesAttacked.Contains(other.gameObject))
                {
                    other.gameObject.GetComponent<EnemyController>().DecreaseHealth(PlayerStats.main.heavyAttackDamage);
                    _enemiesAttacked.Add(other.gameObject);
                }
            }
        }
    }

    public void ClearAttackedEnemies()
    {
        _enemiesAttacked.Clear();
    }
}

/*
 * 
            //making this false because hit already given
            shouldGiveHit = false;
            
            if(PlayerCombat.currentAttackType == AttackType.LightAttack)
                other.gameObject.GetComponent<EnemyController>().DecreaseHealth(PlayerStats.main.lightAttackDamage);
            else if(PlayerCombat.currentAttackType == AttackType.HeavyAttack)
                other.gameObject.GetComponent<EnemyController>().DecreaseHealth(PlayerStats.main.heavyAttackDamage);
*/