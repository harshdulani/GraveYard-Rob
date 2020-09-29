using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float waitBeforeAttacking = 0.75f;

    public bool isAttacking = false;

    private Animator anim;
    private MovementInput movementInput;

    private void Start()
    {
        anim = GetComponent<Animator>();
        movementInput = GetComponent<MovementInput>();
    }

    private void Update()
    {
        if (!isAttacking)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                //rotate to camera.forward
                
                //refactor this to hashid
                anim.SetTrigger("attack1");

                movementInput.blockPlayerRotation = true;
                isAttacking = true;

                //definitely replace this with animation end
                StartCoroutine("WaitBeforeAttacking");

                print("attack initiated");
            }
        }
    }

    private IEnumerator WaitBeforeAttacking()
    {
        yield return new WaitForSeconds(waitBeforeAttacking);
        isAttacking = false;
        movementInput.blockPlayerRotation = false;
        print("Wait ended");
    }
}
