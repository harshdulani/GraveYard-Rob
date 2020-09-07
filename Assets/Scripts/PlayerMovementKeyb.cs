using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementKeyb : MonoBehaviour
{
    public static bool isRunning = false;
    public float maxMovementSpeed = 10f;

    private Vector3 direction;

    //ease in lerping
    private float lerpTime = 1f;
    private float currentLerpTime;

    public float currentMovementSpeed;

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
            currentLerpTime = 0f;
            currentMovementSpeed = 0f;
        }
        else
        {
            isRunning = true;
            animator.SetBool("isRunning", true);

            #region ease in lerping
            //increment timer once per frame
            currentLerpTime += Time.deltaTime * maxMovementSpeed;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            currentLerpTime = 1f - Mathf.Cos(currentLerpTime * Mathf.PI * 0.5f);

            currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, maxMovementSpeed, currentLerpTime);
            #endregion

            transform.Translate(direction * Time.deltaTime * currentMovementSpeed, Space.Self);
        }
    }
}
