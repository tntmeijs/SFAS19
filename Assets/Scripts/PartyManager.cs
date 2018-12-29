using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Joining a party")]
    [SerializeField]
    private KeyCode m_KeyboardJoinButton        = KeyCode.Return;

    [SerializeField]
    private KeyCode m_JoystickOneJoinButton     = KeyCode.Joystick1Button0;

    [SerializeField]
    private KeyCode m_JoystickTwoJoinButton     = KeyCode.Joystick2Button0;

    [SerializeField]
    private KeyCode m_JoystickThreeJoinButton   = KeyCode.Joystick3Button0;

    [SerializeField]
    private KeyCode m_JoystickFourJoinButton    = KeyCode.Joystick4Button0;

    [Header("Leaving a party")]
    [SerializeField]
    private KeyCode m_KeyboardLeaveButton       = KeyCode.Escape;

    [SerializeField]
    private KeyCode m_JoystickOneLeaveButton    = KeyCode.Joystick1Button1;

    [SerializeField]
    private KeyCode m_JoystickTwoLeaveButton    = KeyCode.Joystick2Button1;

    [SerializeField]
    private KeyCode m_JoystickThreeLeaveButton  = KeyCode.Joystick3Button1;

    [SerializeField]
    private KeyCode m_JoystickFourLeaveButton   = KeyCode.Joystick4Button1;

    // --------------------------------------------------------------

    // Maps the controller to the player index
    // List[0] --> player 1
    // List[1] --> player 2
    // List[2] --> player 3
    // List[3] --> player 4
    private List<int> m_RegisteredControllersIndices = new List<int>();

    // A keyboard is considered a controller as well!
    private enum Controllers
    {
        // Starts at -2 to keep the enum mapping simple when converting to integers in the code below
        Invalid = -2,

        Keyboard,   // -1
        Joystick0,  //  0
        Joystick1,  //  1
        Joystick2,  //  2
        Joystick3,  //  3

        TotalNumberOfControllers    // Always equal to the number of controllers if this kind of enum style is used
    }

    // --------------------------------------------------------------

    private void Awake()
    {
        for (int index = 0; index < (int)Controllers.TotalNumberOfControllers; ++index)
        {
            m_RegisteredControllersIndices.Add((int)Controllers.Invalid);
        }
    }

    private void Update()
    {
        PollPartyJoinButtons();
        PollPartyLeaveButtons();
    }

    private void PollPartyJoinButtons()
    {
        if (Input.GetKeyDown(m_KeyboardJoinButton))
        {
            AddGameController(Controllers.Keyboard);
        }

        if (Input.GetKeyDown(m_JoystickOneJoinButton))
        {
            AddGameController(Controllers.Joystick0);
        }

        if (Input.GetKeyDown(m_JoystickTwoJoinButton))
        {
            AddGameController(Controllers.Joystick1);
        }

        if (Input.GetKeyDown(m_JoystickThreeJoinButton))
        {
            AddGameController(Controllers.Joystick2);
        }

        if (Input.GetKeyDown(m_JoystickFourJoinButton))
        {
            AddGameController(Controllers.Joystick3);
        }
    }

    private void AddGameController(Controllers type)
    {
        // The controller that is being added exceeds the maximum number of controllers allowed, no need to continue
        if (m_RegisteredControllersIndices.Count > (int)Controllers.TotalNumberOfControllers)
            return;

        for (int controllerIndex = 0; controllerIndex < (int)Controllers.TotalNumberOfControllers; ++controllerIndex)
        {
            if (m_RegisteredControllersIndices[controllerIndex] == (int)type)
            {
                // This controller exists already, no need to continue
                return;
            }
        }

        // Register the controller index
        for (int controllerIndex = 0; controllerIndex < (int)Controllers.TotalNumberOfControllers; ++controllerIndex)
        {
            if (m_RegisteredControllersIndices[controllerIndex] == (int)Controllers.Invalid)
            {
                m_RegisteredControllersIndices[controllerIndex] = (int)type;
                break;  // Only set one controller, if the others are invalid, leave them for the next controller
            }
        }
    }

    private void PollPartyLeaveButtons()
    {
        if (Input.GetKeyDown(m_KeyboardLeaveButton))
        {
            RemoveGameController(Controllers.Keyboard);
        }

        if (Input.GetKeyDown(m_JoystickOneLeaveButton))
        {
            RemoveGameController(Controllers.Joystick0);
        }

        if (Input.GetKeyDown(m_JoystickTwoLeaveButton))
        {
            RemoveGameController(Controllers.Joystick1);
        }

        if (Input.GetKeyDown(m_JoystickThreeLeaveButton))
        {
            RemoveGameController(Controllers.Joystick2);
        }

        if (Input.GetKeyDown(m_JoystickFourLeaveButton))
        {
            RemoveGameController(Controllers.Joystick3);
        }
    }

    private void RemoveGameController(Controllers type)
    {
        // No more controllers left to unregister
        if (m_RegisteredControllersIndices.Count < 1)
            return;

        for (int controllerIndex = 0; controllerIndex < (int)Controllers.TotalNumberOfControllers; ++controllerIndex)
        {
            if (m_RegisteredControllersIndices[controllerIndex] == (int)type)
            {
                m_RegisteredControllersIndices[controllerIndex] = (int)Controllers.Invalid;
                break;  // Only set one controller, if the others are invalid, leave them for the next controller
            }
        }
    }
}
