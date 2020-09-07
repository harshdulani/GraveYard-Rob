using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsMouse : MonoBehaviour
{
    public Transform targetTransform;

    //ease in lerping
    private float lerpTime = 1f;
    private float currentLerpTime;

    public float easeInSpeed = 1f;
    public static bool shouldRotate = false;

    private float targetAngle = 0f, currentAngle = 0f;

    private void Update()
    {
        if(shouldRotate)
        {
        }
    }
}
