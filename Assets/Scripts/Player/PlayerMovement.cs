using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    public static bool IsMoving = false;
    private FloatingJoystick _joystick;

    public float movementSpeed = 2f;

    //orientation calculation
    //player moves forward by movement speed in the Direction "direction"
    private Vector3 _direction;
    private float _currentAngle = 0f;

    [FormerlySerializedAs("LerpToSnapSpeed")] public float lerpToSnapSpeed = 0.1f;


    private void Start()
    {
        _joystick = FindObjectOfType<FloatingJoystick>();
    }

    private void Update()
    {
        _direction = new Vector3(_joystick.Horizontal, 0f, _joystick.Vertical);

        if (_direction.Equals(Vector3.zero)) //or isn't shooting
        {
            IsMoving = false;
            if (!GameObject.FindGameObjectWithTag("Enemy"))
            {
                _currentAngle = Mathf.LerpAngle(_currentAngle, 0, lerpToSnapSpeed);

                transform.eulerAngles = Vector3.up * _currentAngle;
            }
        }
        else
        {
            IsMoving = true;
            transform.Translate(_direction * Time.deltaTime * movementSpeed, Space.World);

            _currentAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * _currentAngle;
        }
    }
}
