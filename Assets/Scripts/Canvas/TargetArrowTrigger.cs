using UnityEngine;

public class TargetArrowTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        Destroy(gameObject);
    }
}
