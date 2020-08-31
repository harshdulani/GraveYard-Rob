using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private FloatingJoystick joystick;

    public float movementSpeed = 2f;

    //orientation calculation
    private Vector3 direction;
    private float angle;

    private void Start()
    {
        joystick = FindObjectOfType<FloatingJoystick>();
    }

    private void Update()
    {
        var movementVector = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);

        transform.Translate(movementVector * Time.deltaTime * movementSpeed, Space.World);

        float inputAngle = Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.up * inputAngle;
    }
}
