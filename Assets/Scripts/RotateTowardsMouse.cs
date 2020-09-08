using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsMouse : MonoBehaviour
{
    public Transform target;

    //ease in lerping
    private float lerpTime = 1f;
    private float currentLerpTime;

    public float easeInSpeed = 0.05f;
    public static bool shouldRotate = true;

    private Vector3 direction;

    private float targetAngle = 0f, currentAngle = 0f;

    private void Start()
    {
        if (!target)
            target = Camera.main.transform;
    }

    private void Update()
    {
        if(shouldRotate)
        {
            direction = transform.position - target.position ;
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, easeInSpeed);

            transform.rotation = Quaternion.AngleAxis(currentAngle, Vector3.up);
        }
    }
}