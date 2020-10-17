using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float waitBeforeAttacking = 0.75f;
    public bool isAttacking = false;

    private Ray ray;
    private Vector3 position;
    private bool rotatingToPosition = false;
    private Vector3 desiredMovementDirection;

    private int attack1Hash;

    private Animator anim;
    private MovementInput movementInput;
    private Camera cam;

    private void Start()
    {
        anim = GetComponent<Animator>();
        movementInput = GetComponent<MovementInput>();
        cam = Camera.main;

        attack1Hash = Animator.StringToHash("attack1");
    }

    private void Update()
    {
        if (!isAttacking)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                // Reset ray with new mouse position
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                foreach (var hit in Physics.RaycastAll(ray))
                    position = hit.point;  

                //refactor this to hashid
                anim.SetTrigger("attack1");

                //definitely replace this with on animation end
                StartCoroutine("WaitBeforeAttackingAgain");
            }
        }
        
        if(rotatingToPosition)
        {
            Rotate(position);
        }
    }

    private IEnumerator WaitBeforeAttackingAgain()
    {
        isAttacking = true;
        movementInput.playerHasControl = false;
        rotatingToPosition = true;

        yield return new WaitForSeconds(waitBeforeAttacking);

        isAttacking = false;
        movementInput.playerHasControl = true; 
        rotatingToPosition = false;
        position = Vector3.zero;
    }

    public void Rotate(Vector3 position)
    {
        //if using skyrim camera, send v3.zero instead of your camera position
        if (position.Equals(Vector3.zero))
            desiredMovementDirection = transform.position - cam.transform.position;
        else
            desiredMovementDirection = position - transform.position;

        desiredMovementDirection.y = 0f;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMovementDirection), 0.2f);
    }
}
