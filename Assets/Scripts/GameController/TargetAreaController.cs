using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAreaController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerCombat>().allowedToDig = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.GetComponent<PlayerCombat>().allowedToDig = false;
    }
}
