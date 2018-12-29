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
        Invalid = -1,

        A, B, X, Y,

        LeftBumper,
        RightBumper,

        View,
        Menu,

        TotalNumberOfButtons
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

    private const int MAXIMUM_NUMBER_OF_PLAYERS = 4;

    // Maps the controller to the player index
    // List[0] --> player 1
    // List[1] --> player 2
    // List[2] --> player 3
    // List[3] --> player 4
    private List<int> m_RegisteredControllersIndices;

    // A keyboard is considered a controller as well!
    private enum Controllers
    {
        None = -1,

        Keyboard,   // 0
        Joystick1,  // 1
        Joystick2,  // 2
        Joystick3,  // 3
        Joystick4,  // 4

        TotalNumberOfControllers
    }

    // --------------------------------------------------------------

    private void Awake()
    {
        FillControllerSpots();
    }

    private void FillControllerSpots()
    {
        // Allocate enough memory for all players
        m_RegisteredControllersIndices = new List<int>(MAXIMUM_NUMBER_OF_PLAYERS);

        // Set each controller slot to no controller at all
        for (int index = 0; index < MAXIMUM_NUMBER_OF_PLAYERS; ++index)
        {
            m_RegisteredControllersIndices.Add((int) Controllers.None);
        }
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
            AddGameController(Controllers.Keyboard);
        }

        // Joystick 1
        if (Input.GetKeyDown(ConvertJoystickButtonToKeycode((int)Controllers.Joystick1, m_JoystickJoinButton)))
        {
            AddGameController(Controllers.Joystick1);
        }

        // Joystick 2
        if (Input.GetKeyDown(ConvertJoystickButtonToKeycode((int)Controllers.Joystick2, m_JoystickJoinButton)))
        {
            AddGameController(Controllers.Joystick2);
        }

        // Joystick 3
        if (Input.GetKeyDown(ConvertJoystickButtonToKeycode((int)Controllers.Joystick3, m_JoystickJoinButton)))
        {
            AddGameController(Controllers.Joystick3);
        }

        // Joystick 4
        if (Input.GetKeyDown(ConvertJoystickButtonToKeycode((int)Controllers.Joystick4, m_JoystickJoinButton)))
        {
            AddGameController(Controllers.Joystick4);
        }
    }

    private void AddGameController(Controllers type)
    {
        // The controller that is being added exceeds the maximum number of players allowed, no need to continue
        if (m_RegisteredControllersIndices.Count > MAXIMUM_NUMBER_OF_PLAYERS)
            return;

        for (int controllerIndex = 0; controllerIndex < MAXIMUM_NUMBER_OF_PLAYERS; ++controllerIndex)
        {
            if (m_RegisteredControllersIndices[controllerIndex] == (int)type)
            {
                // This controller exists already, no need to continue
                return;
            }
        }

        // Register the controller index
        for (int controllerIndex = 0; controllerIndex < MAXIMUM_NUMBER_OF_PLAYERS; ++controllerIndex)
        {
            if (m_RegisteredControllersIndices[controllerIndex] == (int)Controllers.None)
            {
                m_RegisteredControllersIndices[controllerIndex] = (int)type;
                break;  // Only set one controller, if the others are invalid, leave them for the next controller
            }
        }
    }

    private void PollPartyLeaveButtons()
    {
        // Keyboard
        if (Input.GetKeyDown(m_KeyboardLeaveButton))
        {
            RemoveGameController(Controllers.Keyboard);
        }

        // Joystick 1
        if (Input.GetKeyDown(ConvertJoystickButtonToKeycode((int)Controllers.Joystick1, m_JoystickLeaveButton)))
        {
            RemoveGameController(Controllers.Joystick1);
        }

        // Joystick 2
        if (Input.GetKeyDown(ConvertJoystickButtonToKeycode((int)Controllers.Joystick2, m_JoystickLeaveButton)))
        {
            RemoveGameController(Controllers.Joystick2);
        }

        // Joystick 3
        if (Input.GetKeyDown(ConvertJoystickButtonToKeycode((int)Controllers.Joystick3, m_JoystickLeaveButton)))
        {
            RemoveGameController(Controllers.Joystick3);
        }

        // Joystick 4
        if (Input.GetKeyDown(ConvertJoystickButtonToKeycode((int)Controllers.Joystick4, m_JoystickLeaveButton)))
        {
            RemoveGameController(Controllers.Joystick4);
        }
    }

    private void RemoveGameController(Controllers type)
    {
        // No more controllers left to unregister
        if (m_RegisteredControllersIndices.Count < 1)
            return;

        for (int controllerIndex = 0; controllerIndex < MAXIMUM_NUMBER_OF_PLAYERS; ++controllerIndex)
        {
            if (m_RegisteredControllersIndices[controllerIndex] == (int)type)
            {
                m_RegisteredControllersIndices[controllerIndex] = (int)Controllers.None;
            }
        }
    }

    private KeyCode ConvertJoystickButtonToKeycode(int joystickID, JoystickButton button)
    {
        // Convert to one of the key code enum values
        return (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + joystickID + "Button" + (int)button);
    }
}
