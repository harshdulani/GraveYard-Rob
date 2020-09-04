using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementKeyb : MonoBehaviour
{
    public static bool isMoving = false;
    public float movementSpeed = 2f;

    private Vector3 direction;

    public float jumpForce = 10f;

    private static Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("isJumping");
            //GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
        }

        if (direction.Equals(Vector3.zero)) //or isn't shooting
            isMoving = false;
        else
        {
            isMoving = true;
            transform.Translate(direction * Time.deltaTime * movementSpeed, Space.Self);
        }
    }
}
