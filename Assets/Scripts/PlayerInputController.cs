using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkateboardController))]
public class PlayerInputController : MonoBehaviour
{
    // //////////////////////////////////////////////////////////////////////////
    // Editor-exposed private member variables
    // //////////////////////////////////////////////////////////////////////////
    [SerializeField] private KeyCode m_KeyPushForward               = KeyCode.W;
    [SerializeField] private KeyCode m_KeyAlternativePushForward    = KeyCode.UpArrow;
    [SerializeField] private KeyCode m_KeySteerLeft                 = KeyCode.A;
    [SerializeField] private KeyCode m_KeyAlternativeSteerLeft      = KeyCode.LeftArrow;
    [SerializeField] private KeyCode m_KeySteerRight                = KeyCode.D;
    [SerializeField] private KeyCode m_KeyAlternativeSteerRight     = KeyCode.RightArrow;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_MinimumSpeedForSteeringFactorThreshold = 0.5f;

    [SerializeField] private float m_ForwardPushStrength    = 1000.0f;
    [SerializeField] private float m_SteeringStrength       = 1000.0f;
    [SerializeField] private float m_BrakeStrength          = 100.0f;

    [SerializeField] private float m_ControllerScanTimeoutInSeconds = 5.0f;

    // //////////////////////////////////////////////////////////////////////////
    // Private member variables
    // //////////////////////////////////////////////////////////////////////////
    private SkateboardController m_SkateboardController;

    private int m_NumberOfConnectedControllers = 0;

    private float m_CurrentForwardPushStrength  = 0.0f;
    private float m_CurrentSteeringStrength     = 0.0f;

    private bool m_SteerLeftPressed     = false;
    private bool m_SteerRightPressed    = false;
    private bool m_PushForwardPressed   = false;

    private void Awake()
    {
        m_SkateboardController = GetComponent<SkateboardController>();

        // Since the controller count does not need to update every frame, it is done using a co-routine.
        // This will prevent unnecessary queries of the controller names array.
        StartCoroutine(CheckForControllers());
    }

    private IEnumerator CheckForControllers()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(m_ControllerScanTimeoutInSeconds);

            m_NumberOfConnectedControllers = Input.GetJoystickNames().Length;
        }
    }

    private void Update()
    {
        ProcessPlayerInput();
        ConvertInputToBoardControlValues();
        ApplyBoardControlValues();
    }

    private void ProcessPlayerInput()
    {
        if (m_NumberOfConnectedControllers == 0)
        {
            HandleKeyboardControls();
        }
        else
        {
            HandleJoystickControls();
        }
    }

    private void HandleKeyboardControls()
    {
        m_SteerLeftPressed      = Input.GetKey(m_KeySteerLeft) || Input.GetKey(m_KeyAlternativeSteerLeft);
        m_SteerRightPressed     = Input.GetKey(m_KeySteerRight) || Input.GetKey(m_KeyAlternativeSteerRight);
        m_PushForwardPressed    = Input.GetKey(m_KeyPushForward) || Input.GetKey(m_KeyAlternativePushForward);
    }

    private void HandleJoystickControls()
    {
        Debug.LogWarning("Joystick controls have not been implemented yet!");
    }

    private void ApplyBoardControlValues()
    {
        m_SkateboardController.SteerLeftRight(m_CurrentSteeringStrength);
        m_SkateboardController.PushForward(m_CurrentForwardPushStrength);
    }

    private void ConvertInputToBoardControlValues()
    {
        if (m_PushForwardPressed)
        {
            m_CurrentForwardPushStrength = m_ForwardPushStrength;
        }
        else
        {
            // Gradually decrease the forward speed of the board
            if (m_CurrentForwardPushStrength > 0.0f)
            {
                m_CurrentForwardPushStrength -= m_BrakeStrength * Time.deltaTime;
            }
            else
            {
                m_CurrentForwardPushStrength = 0.0f;
            }
        }

        // Steering is only allowed while in motion
        if ((m_SteerLeftPressed || m_SteerRightPressed) &&
            m_CurrentForwardPushStrength > m_ForwardPushStrength * m_MinimumSpeedForSteeringFactorThreshold)
        {
            m_CurrentSteeringStrength = m_SteerLeftPressed ? -m_SteeringStrength : m_SteeringStrength;

            // The "sharpness" of the final steering angle depends on the speed of the ride, at high speeds, the
            // player will not be able to make sharp turns.
            //m_CurrentSteeringStrength *= 1.0f - (m_ForwardPushStrength / m_CurrentForwardPushStrength);
        }
        else
        {
            m_CurrentSteeringStrength = 0.0f;
        }
    }
}
