using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The party manager is a class that takes care of the main menu set-up. This is what allows players to join a party
/// or leave it. It does not handle the assignment of controllers, however, it is responsible for polling all input from
/// the system. This class heavily relies on the controller manager.
/// </summary>
public class PartyManager : MonoBehaviour
{
    // --------------------------------------------------------------
    
    // Join as a human using the keyboard
    [Header("Join game")]
    [SerializeField]
    private KeyCode m_KeyboardJoinButton = KeyCode.Return;

    // Join as a human using a joystick
    [SerializeField]
    private Global.JoystickButton m_JoystickJoinButton = Global.JoystickButton.A;

    // Leave as a human using the keyboard
    [Header("Leave game")]
    [SerializeField]
    private KeyCode m_KeyboardLeaveButton = KeyCode.Escape;

    // Leave as a human using a joystick
    [SerializeField]
    private Global.JoystickButton m_JoystickLeaveButton = Global.JoystickButton.B;

    [Header("References")]
    // Actually handles the in-game input
    [SerializeField]
    private InputManager m_InputManager = null;

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
    
    public ControllerManager GetControllerManager()
    {
        return m_ControllerManager;
    }

    // --------------------------------------------------------------

    private void Awake()
    {
        CheckIfReferencesAreSet();

        m_ControllerManager.Initialize();
        SaveCustomKeyCodesAsUnityKeyCodes();
        SetControllerManagerCallbacks();
    }

    private void CheckIfReferencesAreSet()
    {
        if (!m_InputManager)
        {
            Debug.LogError("CRITICAL ERROR: Not all references have been set!");
        }
    }

    private void SaveCustomKeyCodesAsUnityKeyCodes()
    {
        // To make the inspector easy to use, a custom joystick button enumeration is used.
        // This does, however, mean that Unity is no longer able to interpret those buttons directory.
        // Therefore, the utility function below has to be used. This converts the custom button enumeration into the
        // Unity enumeration.
        m_JoystickOneJoinCode       = Global.ConvertJoystickButtonToKeycode(Global.Controllers.Joystick1, m_JoystickJoinButton);
        m_JoystickTwoJoinCode       = Global.ConvertJoystickButtonToKeycode(Global.Controllers.Joystick2, m_JoystickJoinButton);
        m_JoystickThreeJoinCode     = Global.ConvertJoystickButtonToKeycode(Global.Controllers.Joystick3, m_JoystickJoinButton);
        m_JoystickFourJoinCode      = Global.ConvertJoystickButtonToKeycode(Global.Controllers.Joystick4, m_JoystickJoinButton);

        m_JoystickOneLeaveCode      = Global.ConvertJoystickButtonToKeycode(Global.Controllers.Joystick1, m_JoystickLeaveButton);
        m_JoystickTwoLeaveCode      = Global.ConvertJoystickButtonToKeycode(Global.Controllers.Joystick2, m_JoystickLeaveButton);
        m_JoystickThreeLeaveCode    = Global.ConvertJoystickButtonToKeycode(Global.Controllers.Joystick3, m_JoystickLeaveButton);
        m_JoystickFourLeaveCode     = Global.ConvertJoystickButtonToKeycode(Global.Controllers.Joystick4, m_JoystickLeaveButton);
    }

    private void SetControllerManagerCallbacks()
    {
        // Every time the controller manager registers a new player, the input manager has its mappings updated
        m_ControllerManager.OnPlayerJoin += playerIndex => {
            // Retrieve the current player controller type
            Global.Controllers type = m_ControllerManager.GetControllerTypeForPlayerWithIndex(playerIndex);

            // Update the input manager so it had the most up-to-date input mapping available
            m_InputManager.SetControllerTypeForPlayer(playerIndex, type);
        };

        // Every time the controller manager unregisters an existing player, the input manager has its mappings updated
        m_ControllerManager.OnPlayerLeave += playerIndex => {
            m_InputManager.SetControllerTypeForPlayer(playerIndex, Global.Controllers.None);
        };
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
            m_ControllerManager.AddGameController(Global.Controllers.Keyboard);
        }

        // Joystick 1
        if (Input.GetKeyDown(m_JoystickOneJoinCode))
        {
            m_ControllerManager.AddGameController(Global.Controllers.Joystick1);
        }

        // Joystick 2
        if (Input.GetKeyDown(m_JoystickTwoJoinCode))
        {
            m_ControllerManager.AddGameController(Global.Controllers.Joystick2);
        }

        // Joystick 3
        if (Input.GetKeyDown(m_JoystickThreeJoinCode))
        {
            m_ControllerManager.AddGameController(Global.Controllers.Joystick3);
        }

        // Joystick 4
        if (Input.GetKeyDown(m_JoystickFourJoinCode))
        {
            m_ControllerManager.AddGameController(Global.Controllers.Joystick4);
        }
    }

    private void PollPartyLeaveButtons()
    {
        // Keyboard
        if (Input.GetKeyDown(m_KeyboardLeaveButton))
        {
            m_ControllerManager.RemoveGameController(Global.Controllers.Keyboard);
        }

        // Joystick 1
        if (Input.GetKeyDown(m_JoystickOneLeaveCode))
        {
            m_ControllerManager.RemoveGameController(Global.Controllers.Joystick1);
        }

        // Joystick 2
        if (Input.GetKeyDown(m_JoystickTwoLeaveCode))
        {
            m_ControllerManager.RemoveGameController(Global.Controllers.Joystick2);
        }

        // Joystick 3
        if (Input.GetKeyDown(m_JoystickThreeLeaveCode))
        {
            m_ControllerManager.RemoveGameController(Global.Controllers.Joystick3);
        }

        // Joystick 4
        if (Input.GetKeyDown(m_JoystickFourLeaveCode))
        {
            m_ControllerManager.RemoveGameController(Global.Controllers.Joystick4);
        }
    }
}
