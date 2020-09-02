using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementKeyb : MonoBehaviour
{
    public static bool isMoving = false;
    public float movementSpeed = 2f;

    private Vector3 direction;

    private void Update()
    {
        direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (direction.Equals(Vector3.zero)) //or isn't shooting
            isMoving = false;
        else
        {
            isMoving = true;
            transform.Translate(direction * Time.deltaTime * movementSpeed, Space.World);
        }
    }
}
