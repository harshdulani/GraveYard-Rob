using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInput : MonoBehaviour
{
    public float movementSpeed;
    public bool isWalking = true;
    public float rotationSlerpSpeed;

    //rotation
    public Vector3 desiredMovementDirection;
    public bool playerHasControl = true;    
    public float allowPlayerRotationSpeed;

    //jumping
    public float jumpSpeed = 7.5f;
    public bool doJump = false;

    //grounding
    public bool isGrounded;
    private bool jumpDone = false;

    private float speed;
    private float inputX, inputZ;
    private Vector3 forward, right;

    //animator hashes for performance++
    private int speedHash, valXHash, valZHash, isMovingHash, startJumpHash;

    //componentss
    private Camera cam;
    private static Animator animator;
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
        animator = GetComponent<Animator>();

        speedHash = Animator.StringToHash("inputMagnitude");
        valXHash = Animator.StringToHash("valX");
        valZHash = Animator.StringToHash("valZ");
        isMovingHash = Animator.StringToHash("isMoving");
        startJumpHash = Animator.StringToHash("startJump");
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;

        if(playerHasControl)
        { 
            if (Input.GetButtonDown("Jump"))
            {
                animator.SetTrigger(startJumpHash);
                doJump = true;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isWalking = true;
            }
            else
                isWalking = false;
            InputMagnitude();
        }
    }

    private void InputMagnitude()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        
        speed = new Vector2(inputX, inputZ).sqrMagnitude;

        if (isWalking)
        {
            speed = Mathf.Clamp(speed, -0.5f, 0.5f);
            inputX = Mathf.Clamp(inputX, -0.75f, 0.75f);
        }

        //sending input values to animator
        //the third value is damping, set for blending on keyboards
        animator.SetFloat(valXHash, inputX, 0.1f, Time.deltaTime * 2f);
        animator.SetFloat(valZHash, inputZ, 0.1f, Time.deltaTime * 2f);

        animator.SetFloat(speedHash, speed);

        if (speed > allowPlayerRotationSpeed)
        {
            PlayerMoveAndRotate();
        }
        else
        {
            animator.SetBool(isMovingHash, false);
        }
    }

    private void PlayerMoveAndRotate()
    {
        forward = cam.transform.forward;
        right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        desiredMovementDirection = forward * inputZ * movementSpeed + right * inputX * movementSpeed;

        if (isWalking)
            desiredMovementDirection *= 0.15f;


        if (!desiredMovementDirection.Equals(Vector3.zero))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMovementDirection), rotationSlerpSpeed);
            animator.SetBool(isMovingHash, true);
        }

        if (doJump)
        {
            desiredMovementDirection += (Vector3.up * jumpSpeed);
            doJump = false;
            jumpDone = true;
        }

        if(jumpDone)
        {
            if (!isGrounded)
            {
                if (desiredMovementDirection.y >= 0.5f)
                    desiredMovementDirection.y = -desiredMovementDirection.y;
            }
            else
            {
                jumpDone = false;
            }
        }

        //not using delta time made the movement speed dependent on screen size (easy render high fps)
        controller.Move(desiredMovementDirection * Time.deltaTime);
    }
}
