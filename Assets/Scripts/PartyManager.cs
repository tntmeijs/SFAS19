using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    // --------------------------------------------------------------

    // This mapping assumes an XboxOne controller
    private enum JoystickButton
    {
        A, B, X, Y,

        LeftBumper,
        RightBumper,

        View,   // Called "back" on Xbox360
        Menu,   // Called "start" on Xbox360
    }

    // Join as a human using the keyboard
    [Header("Join game")]
    [SerializeField]
    private KeyCode m_KeyboardJoinButton = KeyCode.Return;

    // Join as a human using a joystick
    [SerializeField]
    private JoystickButton m_JoystickJoinButton = JoystickButton.A;

    // Leave as a human using the keyboard
    [Header("Leave game")]
    [SerializeField]
    private KeyCode m_KeyboardLeaveButton = KeyCode.Escape;

    // Leave as a human using a joystick
    [SerializeField]
    private JoystickButton m_JoystickLeaveButton = JoystickButton.B;

    // --------------------------------------------------------------

    // Manages all controllers
    private ControllerManager m_ControllerManager = new ControllerManager();

    // Buttons that will be polled each frame
    private KeyCode m_JoystickOneJoinCode;
    private KeyCode m_JoystickTwoJoinCode;
    private KeyCode m_JoystickThreeJoinCode;
    private KeyCode m_JoystickFourJoinCode;

    private KeyCode m_JoystickOneLeaveCode;
    private KeyCode m_JoystickTwoLeaveCode;
    private KeyCode m_JoystickThreeLeaveCode;
    private KeyCode m_JoystickFourLeaveCode;

    // --------------------------------------------------------------

    private void Awake()
    {
        m_ControllerManager.Initialize();
        SaveCustomKeyCodesAsUnityKeyCodes();
    }

    private void SaveCustomKeyCodesAsUnityKeyCodes()
    {
        // To make the inspector easy to use, a custom joystick button enumeration is used.
        // This does, however, mean that Unity is no longer able to interpret those buttons directory.
        // Therefore, the utility function below has to be used. This converts the custom button enumeration into the
        // Unity enumeration.
        m_JoystickOneJoinCode       = ConvertJoystickButtonToKeycode(ControllerManager.Controllers.Joystick1, m_JoystickJoinButton);
        m_JoystickTwoJoinCode       = ConvertJoystickButtonToKeycode(ControllerManager.Controllers.Joystick2, m_JoystickJoinButton);
        m_JoystickThreeJoinCode     = ConvertJoystickButtonToKeycode(ControllerManager.Controllers.Joystick3, m_JoystickJoinButton);
        m_JoystickFourJoinCode      = ConvertJoystickButtonToKeycode(ControllerManager.Controllers.Joystick4, m_JoystickJoinButton);

        m_JoystickOneLeaveCode      = ConvertJoystickButtonToKeycode(ControllerManager.Controllers.Joystick1, m_JoystickLeaveButton);
        m_JoystickTwoLeaveCode      = ConvertJoystickButtonToKeycode(ControllerManager.Controllers.Joystick2, m_JoystickLeaveButton);
        m_JoystickThreeLeaveCode    = ConvertJoystickButtonToKeycode(ControllerManager.Controllers.Joystick3, m_JoystickLeaveButton);
        m_JoystickFourLeaveCode     = ConvertJoystickButtonToKeycode(ControllerManager.Controllers.Joystick4, m_JoystickLeaveButton);
    }

    private KeyCode ConvertJoystickButtonToKeycode(ControllerManager.Controllers joystickID, JoystickButton button)
    {
        // Convert to one of the key code enum values (it is safe to assume that the Unity enumeration names will never
        // change. This is why the hard-coded values are justified. If, for some reason, these value change in the future,
        // please expose them to the inspector. However, they have been the same for the last few years, so there is no
        // real reason to assume that this will change anytime soon.
        return (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + (int)joystickID + "Button" + (int)button);
    }

    private void Update()
    {
        // Not ideal, but at the point of writing this code, the new Unity input system is still in Beta.
        // Switching to the new system once it releases (Q1 2019) is not needed right away, as this code does its job
        // and is not dragging down the overall performance a lot. No need to spend time on redoing this as long as it
        // still works properly.
        PollPartyJoinButtons();
        PollPartyLeaveButtons();
    }

    private void PollPartyJoinButtons()
    {
        // Keyboard
        if (Input.GetKeyDown(m_KeyboardJoinButton))
        {
            m_ControllerManager.AddGameController(ControllerManager.Controllers.Keyboard);
        }

        // Joystick 1
        if (Input.GetKeyDown(m_JoystickOneJoinCode))
        {
            m_ControllerManager.AddGameController(ControllerManager.Controllers.Joystick1);
        }

        // Joystick 2
        if (Input.GetKeyDown(m_JoystickTwoJoinCode))
        {
            m_ControllerManager.AddGameController(ControllerManager.Controllers.Joystick2);
        }

        // Joystick 3
        if (Input.GetKeyDown(m_JoystickThreeJoinCode))
        {
            m_ControllerManager.AddGameController(ControllerManager.Controllers.Joystick3);
        }

        // Joystick 4
        if (Input.GetKeyDown(m_JoystickFourJoinCode))
        {
            m_ControllerManager.AddGameController(ControllerManager.Controllers.Joystick4);
        }
    }

    private void PollPartyLeaveButtons()
    {
        // Keyboard
        if (Input.GetKeyDown(m_KeyboardLeaveButton))
        {
            m_ControllerManager.RemoveGameController(ControllerManager.Controllers.Keyboard);
        }

        // Joystick 1
        if (Input.GetKeyDown(m_JoystickOneLeaveCode))
        {
            m_ControllerManager.RemoveGameController(ControllerManager.Controllers.Joystick1);
        }

        // Joystick 2
        if (Input.GetKeyDown(m_JoystickTwoLeaveCode))
        {
            m_ControllerManager.RemoveGameController(ControllerManager.Controllers.Joystick2);
        }

        // Joystick 3
        if (Input.GetKeyDown(m_JoystickThreeLeaveCode))
        {
            m_ControllerManager.RemoveGameController(ControllerManager.Controllers.Joystick3);
        }

        // Joystick 4
        if (Input.GetKeyDown(m_JoystickFourLeaveCode))
        {
            m_ControllerManager.RemoveGameController(ControllerManager.Controllers.Joystick4);
        }
    }
}
