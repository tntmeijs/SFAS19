using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // --------------------------------------------------------------

    // Offset of the camera relative to the car
    private Vector3 m_CameraOffset = Vector3.zero;

    // The Camera Target
    private Transform m_PlayerTransform;

    // Time it takes to move to the target position
    private float m_MovementInterpolationSpeed;

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

    public void SetCameraMovementInterpolationSpeed(float speed)
    {
        m_MovementInterpolationSpeed = speed;
    }

    // --------------------------------------------------------------

    // Camera updates are handled here to ensure that all movement for this frame has been done already
    private void LateUpdate()
    {
        // Smoothly follow the car
        transform.position = Vector3.Lerp(transform.position, m_PlayerTransform.TransformPoint(m_CameraOffset), Time.deltaTime * m_MovementInterpolationSpeed);

        // Keep the camera pointed to the car
        transform.LookAt(m_PlayerTransform);
    }
}
