using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public bool shouldGiveHit;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Enemy")) return;

        if (!shouldGiveHit) return;
        
        Vector3 playerForward = MovementInput.current.transform.TransformDirection(Vector3.forward);
        Vector3 toOther = other.transform.position - MovementInput.current.transform.position;
                
        if (Vector3.Dot(playerForward.normalized, toOther.normalized) > 0)
        {
            //making this false because hit already given
            shouldGiveHit = false;
            
            if(PlayerCombat.currentAttackType == AttackType.LightAttack)
                other.gameObject.GetComponent<EnemyController>().DecreaseHealth(PlayerStats.LightAttackDamage);
            else if(PlayerCombat.currentAttackType == AttackType.HeavyAttack)
                other.gameObject.GetComponent<EnemyController>().DecreaseHealth(PlayerStats.HeavyAttackDamage);
        }
    }
}