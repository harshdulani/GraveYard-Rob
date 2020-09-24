using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementKeyb : MonoBehaviour
{
    public static bool isRunning = false;

    private static Animator animator;

    private float inputX, inputZ;
    private Vector3 direction;

    //ease in lerping movement
    public float currentMovementSpeed, desiredMovementSpeed;

    private float lerpTime = 0.1f;

    //lerping of rotation
    public float rotationLerpSpeed = 0.1f;

    private float targetAngle, currentAngle;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        direction = new Vector3(inputX, 0f, inputZ);

        if (Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("startJump"); 
            RotateTowardsMouse.shouldRotate = false;
        }

        if (direction.Equals(Vector3.zero))     //or isn't shooting
        {
            isRunning = false;
            animator.SetBool("isMoving", false);
            currentMovementSpeed = 0f;
        }
        else
        {
            isRunning = true;
            RotateTowardsMouse.shouldRotate = true;
            animator.SetBool("isMoving", true);
            animator.SetFloat("valX", inputX);
            animator.SetFloat("valZ", inputZ);            

            //PLAYER ROTATION

            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationLerpSpeed);

            transform.rotation = Quaternion.AngleAxis(currentAngle, transform.up);

            //PLAYER MOVEMENT

            desiredMovementSpeed = new Vector2(inputX, inputZ).sqrMagnitude;

            currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, desiredMovementSpeed, lerpTime);
            //idk what this does
            animator.SetFloat("inputMagnitude", desiredMovementSpeed);

            GetComponent<CharacterController>().Move(direction * Time.deltaTime * currentMovementSpeed);
        }
    }
}
