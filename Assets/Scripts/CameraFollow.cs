﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // --------------------------------------------------------------

    // Offset of the camera relative to the car
    private Vector3 m_CameraOffset = Vector3.zero;

    // The Camera Target
    private Transform m_PlayerTransform;

    // Multiplier for Time.deltaTime when linear interpolating
    private float m_InterpolationSpeed;

    // --------------------------------------------------------------

    // Directly set the camera follow target
    public void SetTargetTransform(Transform target)
    {
        m_PlayerTransform = target;
    }

    // Directly set the camera offset
    public void SetCameraTargetOffset(Vector3 offset)
    {
        m_CameraOffset = offset;
    }

    public void SetCameraInterpolationSpeed(float speed)
    {
        m_InterpolationSpeed = speed;
    }

    // --------------------------------------------------------------

    // Camera updates are handled here to ensure that all movement for this frame has been done already
    private void LateUpdate()
    {
        // Smoothly follow the car
        transform.position = Vector3.Lerp(transform.position, m_PlayerTransform.position + m_CameraOffset, Time.deltaTime * m_InterpolationSpeed);
    }
}
