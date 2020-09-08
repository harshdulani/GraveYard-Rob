using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementKeyb : MonoBehaviour
{
    public static bool isRunning = false;
    public float maxMovementSpeed = 10f;

    //idle mgmt, to enable rotating towards camera
    public float maxIdleTime = 0f;
    private float currentIdleTime = 0f;

    private static Animator animator;

    private Vector3 direction;

    //ease in lerping movement
    private float lerpTime = 1f;
    private float currentLerpTime;

    public float currentMovementSpeed;

    public float rotationLerpSpeed = 0.1f;
    private float targetAngle = 0f, currentAngle = 0f;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("isJumping"); 
            RotateTowardsMouse.shouldRotate = false;
        }

        if (direction.Equals(Vector3.zero))     //or isn't shooting
        {
            isRunning = false;
            animator.SetBool("isRunning", false);
            currentLerpTime = 0f;
            currentMovementSpeed = 0f;
        }
        else
        {
            isRunning = true;
            RotateTowardsMouse.shouldRotate = true;
            animator.SetBool("isRunning", true);
            animator.SetFloat("valX", Input.GetAxis("Horizontal"));
            animator.SetFloat("valZ", Input.GetAxis("Vertical"));

            //PLAYER ROTATION
            /*
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (targetAngle == 180f || targetAngle == -180f)
                targetAngle = 0f;

            currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationLerpSpeed);

            transform.rotation = Quaternion.AngleAxis(currentAngle, transform.up);*/

            //PLAYER MOVEMENT

            #region ease in lerping movement
            //increment timer once per frame
            currentLerpTime += Time.deltaTime * maxMovementSpeed;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            currentLerpTime = 1f - Mathf.Cos(currentLerpTime * Mathf.PI * 0.5f);

            currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, maxMovementSpeed, currentLerpTime);
            #endregion

            //figure this out, this might be key for implementing run fwd and run bckward.
            //okay so most probably you need to make rotations to the player based on the MainCamera's transform
            //so

            transform.Translate(direction * Time.deltaTime * currentMovementSpeed, Space.Self);
        }
    }
}
