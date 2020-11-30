using System;
using UnityEngine;

public class FindTargetGrave : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) return;
        
        GameFlowEvents.current.updateObjective();
        Destroy(gameObject);
    }
}
