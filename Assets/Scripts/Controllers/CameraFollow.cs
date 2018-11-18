using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Camera settings")]
    [SerializeField] private Vector3 m_CameraOffset = new Vector3(0.0f, 7.5f, -5.5f);

    [SerializeField] private float m_InterpolationSpeed = 5.0f;

    [SerializeField] private List<Transform> m_CameraFollowTargets = new List<Transform>();

    [Header("Cursor settings")]
    [SerializeField] private CursorLockMode m_CursorLockState = CursorLockMode.None;

    [SerializeField] private bool m_EnableCursorVisibility = true;

    // --------------------------------------------------------------

    private void Awake()
    {
        ApplyCursorSettings();
    }

    private void ApplyCursorSettings()
    {
        Cursor.lockState = m_CursorLockState;
        Cursor.visible = m_EnableCursorVisibility;
    }

    private void LogFatalError(string message)
    {
        Debug.LogError("FATAL ERROR: " + message);
    }

    private void Update ()
    {
        // The average position of all targets is the center of the camera
        Vector3 centerOfFocus = GetAveragePositionOfTargets();

        transform.position = Vector3.Lerp(transform.position, centerOfFocus + m_CameraOffset, Time.deltaTime * m_InterpolationSpeed);
    }

    private Vector3 GetAveragePositionOfTargets()
    {
        Vector3 averagePosition = Vector3.zero;

        foreach (Transform transform in m_CameraFollowTargets)
        {
            averagePosition += transform.position;
        }

        return averagePosition / m_CameraFollowTargets.Count;
    }
}
