using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Input settings")]
    [SerializeField] private string m_NameMouseHorizontalInputAxis = "Mouse X";
    [SerializeField] private string m_NameMouseVerticalInputAxis = "Mouse Y";

    [Header("Camera arm references")]
    [SerializeField] private Transform m_YawTransform = null;
    [SerializeField] private Transform m_PitchTransform = null;

    [SerializeField] private Camera m_PlayerCamera = null;

    [Header("Camera settings")]
    [SerializeField] private bool m_InvertYaw = false;
    [SerializeField] private bool m_InvertPitch = false;

    [Range(0.0f, -90.0f)]
    [SerializeField] private float m_LowestPitchAngle = -80.0f;

    [Range(0.0f, 90.0f)]
    [SerializeField] private float m_HighestPitchAngle = 80.0f;

    [Header("Camera sensitivity")]
    [Range(1.0f, 100.0f)]
    [SerializeField] private float m_YawSensitivity = 1.0f;

    [Range(1.0f, 100.0f)]
    [SerializeField] private float m_PitchSensitivity = 1.0f;

    // --------------------------------------------------------------

    private float m_MouseX = 0.0f;
    private float m_MouseY = 0.0f;

    // --------------------------------------------------------------

    private void Awake()
    {
        HideCursor();
        ScalePitchClampValuesToSensitivity();

        bool cameraArmReferencesSetCorrectly = CheckCameraArmReferencesForNull();

        if (!cameraArmReferencesSetCorrectly)
        {
            LogFatalError("Not all camera arm references have been set-up!");
        }
    }

    private void ScalePitchClampValuesToSensitivity()
    {
        m_LowestPitchAngle /= m_PitchSensitivity;
        m_HighestPitchAngle /= m_PitchSensitivity;
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private bool CheckCameraArmReferencesForNull()
    {
        if (!m_YawTransform)
        {
            return false;
        }

        if (!m_PitchTransform)
        {
            return false;
        }

        if (!m_PlayerCamera)
        {
            return false;
        }

        return true;
    }

    private void LogFatalError(string message)
    {
        Debug.LogError("FATAL ERROR: " + message);
    }

	private void Update ()
    {
        GatherMouseInputValues();
        ClampYaw();
        ClampPitch();
        ApplyYawToCameraArm();
        ApplyPitchToCameraArm();
    }
    
    private void GatherMouseInputValues()
    {
        GetYawInput();
        GetPitchInput();
    }

    private void GetYawInput()
    {
        if (m_InvertYaw)
        {
            m_MouseX += Input.GetAxis(m_NameMouseHorizontalInputAxis) * Time.deltaTime;
        }
        else
        {
            m_MouseX -= Input.GetAxis(m_NameMouseHorizontalInputAxis) * Time.deltaTime;
        }
    }

    private void GetPitchInput()
    {
        if (m_InvertPitch)
        {
            m_MouseY += Input.GetAxis(m_NameMouseVerticalInputAxis) * Time.deltaTime;
        }
        else
        {
            m_MouseY -= Input.GetAxis(m_NameMouseVerticalInputAxis) * Time.deltaTime;
        }
    }

    private void ClampYaw()
    {
        if (m_MouseX > 360.0f)
        {
            m_MouseX -= 360.0f;
        }
        else if (m_MouseX < 0.0f)
        {
            m_MouseX += 360.0f;
        }
    }

    private void ClampPitch()
    {
        m_MouseY = Mathf.Clamp(m_MouseY, m_LowestPitchAngle, m_HighestPitchAngle);
    }

    private void ApplyYawToCameraArm()
    {
        m_YawTransform.localRotation = Quaternion.Euler(0.0f, m_MouseX * m_YawSensitivity, 0.0f);
    }

    private void ApplyPitchToCameraArm()
    {
        m_PitchTransform.localRotation = Quaternion.Euler(m_MouseY * m_PitchSensitivity, 0.0f, 0.0f);
    }
}
