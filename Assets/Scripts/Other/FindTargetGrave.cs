using UnityEngine;

public class FindTargetGrave : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) return;
        
        MovementInput.current.GetComponent<PlayerController>().ForceStaminaRegen();
        GameFlowEvents.current.InvokeUpdateObjective();

        Destroy(gameObject);
    }
}
