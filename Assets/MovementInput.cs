﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInput : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSlerpSpeed;

    public Vector3 desiredMovementDirection;
    public bool blockRotationPlayer;    
    public float speed;
    public float allowPlayerRotationSpeed;
    public bool isGrounded;

    public float inputX, inputZ;
    private float tempRotateAngle;
    private float verticalVelocity;
    private Vector3 moveVector;
    private Vector3 forward, right;

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
        InputMagnitude();

        isGrounded = controller.isGrounded;

        if(!isGrounded)
        {
            verticalVelocity -= 2f;
            moveVector = Vector3.up * verticalVelocity;
            controller.Move(moveVector);
        }
    }

    private void PlayerMoveAndRotate()
    {
        forward = cam.transform.forward;
        right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        //forward.Normalize();
        //right.Normalize();

        desiredMovementDirection = forward * inputZ * movementSpeed + right * inputX * movementSpeed;

        if(!blockRotationPlayer)
        {
            if(!desiredMovementDirection.Equals(Vector3.zero))
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMovementDirection), rotationSlerpSpeed);
            {
                tempRotateAngle = Mathf.LerpAngle(transform.eulerAngles.y, Quaternion.LookRotation(desiredMovementDirection).eulerAngles.y, rotationSlerpSpeed);

                transform.rotation = Quaternion.AngleAxis(tempRotateAngle, Vector3.up);
            }
        }
        controller.SimpleMove(desiredMovementDirection);
    }

    private void InputMagnitude()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        //sending input values to animator
        //the third value is damping, set for blending on keyboards
        animator.SetFloat("valX", inputX, 0.0f, Time.deltaTime * 2f);
        animator.SetFloat("valZ", inputZ, 0.0f, Time.deltaTime * 2f);

        speed = new Vector2(inputX, inputZ).sqrMagnitude;

        animator.SetFloat("inputMagnitude", speed);
        if (speed >= allowPlayerRotationSpeed)
        {
            PlayerMoveAndRotate();
        }
    }
}
