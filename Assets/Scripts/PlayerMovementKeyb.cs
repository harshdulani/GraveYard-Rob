using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementKeyb : MonoBehaviour
{
    public static bool isRunning = false;
    public float movementSpeed = 2f;

    private Vector3 direction;

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
        }

        if (direction.Equals(Vector3.zero))     //or isn't shooting
        {
            isRunning = false;
            animator.SetBool("isRunning", false);
        }
        else
        {
            isRunning = true;
            animator.SetBool("isRunning", true);
            transform.Translate(direction * Time.deltaTime * movementSpeed, Space.Self);
        }
    }
}
