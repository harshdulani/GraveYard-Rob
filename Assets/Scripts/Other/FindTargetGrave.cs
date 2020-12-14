using UnityEngine;

public class FindTargetGrave : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) return;
        
        GameFlowEvents.current.InvokeUpdateObjective();
        Destroy(gameObject);
    }
}
