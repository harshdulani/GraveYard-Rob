using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInput : MonoBehaviour
{
    public bool dontLookBack;
    public float movementSpeed;
    public float rotationSlerpSpeed;

    //rotation
    public Vector3 desiredMovementDirection;
    public bool blockRotationPlayer;    
    public float allowPlayerRotationSpeed;

    //grounding
    public bool isGrounded;
    public float verticalVelocity;

    //jumping
    public float jumpSpeed = 7.5f;
    public bool doJump = false;

    private float speed;
    private float inputX, inputZ;
    private Vector3 forward, right;

    //components
    private Camera cam;
    private static Animator animator;
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;
        if (Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("startJump");
            doJump = true;
        }
        InputMagnitude();
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

        if (!blockRotationPlayer)
        {
            if (!desiredMovementDirection.Equals(Vector3.zero))
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMovementDirection), rotationSlerpSpeed);
            }
        }

        //okay i think jumping is still using height of baked animation
        if (doJump)
        {
            //lava mein it used verlocity, so try velocity
            desiredMovementDirection += (Vector3.up * jumpSpeed);
            print(desiredMovementDirection);
            doJump = false;
        }
        else
        {
            //create a variable to check if a jump has landed and only then ground it
            if (!isGrounded)
            {
                verticalVelocity = controller.velocity.y - 0.5f;
                desiredMovementDirection += Vector3.up * verticalVelocity;
            }
        }

        if (desiredMovementDirection.y >= jumpSpeed)
            desiredMovementDirection.y = -desiredMovementDirection.y;        

        //not using delta time made the movement speed dependent on screen size (small screen high fps)
        controller.Move(desiredMovementDirection * Time.deltaTime);
        animator.SetBool("isMoving", true);
    }

    private void InputMagnitude()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        //sending input values to animator
        //the third value is damping, set for blending on keyboards
        animator.SetFloat("valX", inputX, 0.1f, Time.deltaTime * 2f);
        animator.SetFloat("valZ", inputZ, 0.1f, Time.deltaTime * 2f);

        speed = new Vector2(inputX, inputZ).sqrMagnitude;

        animator.SetFloat("inputMagnitude", speed);

        if (speed > allowPlayerRotationSpeed)
        {
            PlayerMoveAndRotate();
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }
}
