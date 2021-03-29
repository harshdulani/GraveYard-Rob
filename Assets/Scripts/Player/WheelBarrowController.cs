using System;
using UnityEngine;

public class WheelBarrowController : MonoBehaviour
{
    #region Singleton

    public static WheelBarrowController main;

    private void Awake()
    {
        if (!main)
            main = this;
        else
            Destroy(main);
    }
    

    #endregion

    public bool isLifted;
    public bool hasMaxGold, canLift;
    
    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) return;
        if(isLifted) return;
        if(!hasMaxGold) return;

        canLift = true;

        //allow pressing E to lift/drop the cart

        //if presses E,
        //new rotation from the wheel as pivot
        //change animation to wheelbarrow walk
        //disable combat
    }

    private void OnTriggerStay(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) return;

        if(!canLift) return;
        
        print("yup");
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) return;

        canLift = false;
    }
}
