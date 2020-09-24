using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementKeyb : MonoBehaviour
{
    public bool dontLookBack;
    public static bool isRunning = false;
    public float maxMovementSpeed = 10f;

    private static Animator animator;

    private float inputX, inputZ;
    private Vector3 direction;

    //ease in lerping movement
    public float currentMovementSpeed;

    private float lerpTime = 1f;
    private float currentLerpTime;

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
        direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("startJump"); 
            RotateTowardsMouse.shouldRotate = false;
        }

        if (direction.Equals(Vector3.zero))     //or isn't shooting
        {
            isRunning = false;
            animator.SetBool("isMoving", false);
            currentLerpTime = 0f;
            currentMovementSpeed = 0f;
        }
        else
        {
            isRunning = true;
            RotateTowardsMouse.shouldRotate = true;
            animator.SetBool("isMoving", true);
            animator.SetFloat("valX", inputX);
            animator.SetFloat("valZ", inputZ);

            var speed = new Vector2(inputX, inputZ).sqrMagnitude;

            animator.SetFloat("inputMagnitude", speed);

            //PLAYER ROTATION

            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if(dontLookBack)
                if (inputZ < 0f)
                    targetAngle = 0f;

            currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationLerpSpeed);

            transform.rotation = Quaternion.AngleAxis(currentAngle, transform.up);

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

            GetComponent<CharacterController>().Move(direction * Time.deltaTime * currentMovementSpeed);
        }
    }
}
