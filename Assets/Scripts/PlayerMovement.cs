using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static bool isMoving = false;
    private FloatingJoystick joystick;

    public float movementSpeed = 2f;

    //orientation calculation
    //player moves forward by movement speed in the Direction "direction"
    private Vector3 direction;
    private float currentAngle = 0f;

    public float LerpToSnapSpeed = 0.1f;


    private void Start()
    {
        joystick = FindObjectOfType<FloatingJoystick>();
    }

    private void Update()
    {
        direction = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);

        if (direction.Equals(Vector3.zero)) //or isn't shooting
        {
            isMoving = false;
            if (!GameObject.FindGameObjectWithTag("Enemy"))
            {
                currentAngle = Mathf.Lerp(currentAngle, 0, LerpToSnapSpeed);

                transform.eulerAngles = Vector3.up * currentAngle;
            }
        }
        else
        {
            isMoving = true;
            transform.Translate(direction * Time.deltaTime * movementSpeed, Space.World);

            currentAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * currentAngle;
        }
    }
}
