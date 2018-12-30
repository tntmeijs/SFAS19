using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // --------------------------------------------------------------

    public static InputManager instance = null;

    // --------------------------------------------------------------

    // Key bindings for keyboard players
    [Header("Keyboard bindings")]
    [SerializeField]
    private KeyCode m_KeyboardThrottle = KeyCode.LeftShift;

    [SerializeField]
    private KeyCode m_KeyboardBrake = KeyCode.LeftControl;

    [SerializeField]
    private KeyCode m_KeyboardSteerLeft = KeyCode.A;

    [SerializeField]
    private KeyCode m_KeyboardSteerRight = KeyCode.D;

    [SerializeField]
    private KeyCode m_KeyboardUp = KeyCode.W;

    [SerializeField]
    private KeyCode m_KeyboardDown = KeyCode.S;

    [SerializeField]
    private KeyCode m_KeyboardActivatePowerUp = KeyCode.Space;

    // Key syntactic sugar bindings for joystick players (converting to Unity bindings will be done when polling input)
    [Header("Joystick bindings")]
    [SerializeField]
    private Global.JoystickButton m_JoystickThrottle = Global.JoystickButton.A;

    [SerializeField]
    private Global.JoystickButton m_JoystickBrake = Global.JoystickButton.B;

    [SerializeField]
    private Global.JoystickAxis m_JoystickSteer = Global.JoystickAxis.LeftStickHorizontal;

    [SerializeField]
    private Global.JoystickAxis m_JoystickUpDown = Global.JoystickAxis.LeftStickVertical;

    [SerializeField]
    private Global.JoystickButton m_JoystickActivatePowerUp = Global.JoystickButton.RightBumper;

    [Header("Input manager data")]
    // Prefix for the horizontal left joystick axis name
    [SerializeField]
    private string m_JoystickLeftStickHorizontalAxisName = "JoyHorizontalLS";

    // Prefix for the vertical left joystick axis name
    [SerializeField]
    private string m_JoystickLeftStickVerticalAxisName = "JoyVerticalLS";

    // Prefix for the horizontal left joystick axis name
    [SerializeField]
    private string m_JoystickRightStickHorizontalAxisName = "JoyHorizontalRS";

    // Prefix for the vertical Right joystick axis name
    [SerializeField]
    private string m_JoystickRightStickVerticalAxisName = "JoyVerticalRS";

    // Prefix for the left trigger joystick axis name
    [SerializeField]
    private string m_JoystickLeftTriggerAxisName = "JoyTriggerL";

    // Prefix for the right trigger joystick axis name
    [SerializeField]
    private string m_JoystickRightTriggerAxisName = "JoyTriggerR";

    // Prefix for the d-pad horizontal joystick axis name
    [SerializeField]
    private string m_JoystickDPadHorizontalAxisName = "JoyDPadHorizontal";

    // Prefix for the d-pad vertical joystick axis name
    [SerializeField]
    private string m_JoystickDPadVerticalAxisName = "JoyDPadVertical";

    // --------------------------------------------------------------

    // The order matters, index 0 maps to player one, index 1 map to player 2, etc...
    private Global.Controllers[] m_PlayerControllers = new Global.Controllers[Global.MAXIMUM_NUMBER_OF_PLAYERS];

    // --------------------------------------------------------------

    public void SetControllerTypeForPlayer(Global.Player player, Global.Controllers type)
    {
        m_PlayerControllers[(int)player] = type;
    }

    public Global.PlayerInputData GetInputDataForPlayer(Global.Player player)
    {
        Global.PlayerInputData inputData = new Global.PlayerInputData();
        Global.Controllers controller = m_PlayerControllers[(int)player];

        // Save the type of controller
        inputData.controller = controller;

        // This player slot has not controller assigned to it
        if (controller == Global.Controllers.None)
            return inputData;

        // Handle the type of controller accordingly
        if (controller == Global.Controllers.Keyboard)
            HandleKeyboardInput(ref inputData);
        else
            HandleJoystickInput(ref inputData, controller);

        return inputData;
    }

    // --------------------------------------------------------------

    private void Awake()
    {
        EnforceSingleton();
        ResetAllControllerEntries();
    }

    private void EnforceSingleton()
    {
        if (instance == null)
        {
            // No instance exists yet
            instance = this;
        }
        else if (instance != this)
        {
            // Instance exists already, destroy this new instance
            Destroy(gameObject);
        }

        // Make the input manager persist during scene (re)loading
        DontDestroyOnLoad(gameObject);
    }

    private void ResetAllControllerEntries()
    {
        for (int controllerIndex = 0; controllerIndex < m_PlayerControllers.Length; ++controllerIndex)
        {
            m_PlayerControllers[controllerIndex] = Global.Controllers.None;
        }
    }
    
    private void HandleKeyboardInput(ref Global.PlayerInputData data)
    {
        data.buttonA = PollButton(m_KeyboardThrottle);
        data.buttonB = PollButton(m_KeyboardBrake);
        data.buttonRightBumper = PollButton(m_KeyboardActivatePowerUp);

        bool steerLeft = PollButton(m_KeyboardSteerLeft);
        bool steerRight = PollButton(m_KeyboardSteerRight);

        bool up = PollButton(m_KeyboardUp);
        bool down = PollButton(m_KeyboardDown);

        // Since the keyboard input is not based on axes, the individual inputs have to be combined into a single
        // floating-point value for the steering value.
        data.axisLeftStickHorizontal = ConvertBooleansToAxisValue(steerLeft, steerRight);
        data.axisLeftStickVertical = ConvertBooleansToAxisValue(down, up);
    }

    private void HandleJoystickInput(ref Global.PlayerInputData data, Global.Controllers joystick)
    {
        data.buttonA = PollButton(Global.ConvertJoystickButtonToKeycode(joystick, m_JoystickThrottle));
        data.buttonB = PollButton(Global.ConvertJoystickButtonToKeycode(joystick, m_JoystickBrake));
        data.buttonRightBumper = PollButton(Global.ConvertJoystickButtonToKeycode(joystick, m_JoystickActivatePowerUp));

        data.axisLeftStickHorizontal = PollAxis(ConstructAxisNameFromCustomAxisTypeAndJoystick(joystick, m_JoystickSteer));
        data.axisLeftStickVertical = PollAxis(ConstructAxisNameFromCustomAxisTypeAndJoystick(joystick, m_JoystickUpDown));
    }

    private bool PollButton(KeyCode keyCode)
    {
        if (Input.GetKey(keyCode))
            return true;
        else
            return false;
    }

    private float PollAxis(string axisName)
    {
        return Input.GetAxis(axisName);
    }

    // Convert two Boolean inputs into a range that can be used to simulate a joystick axis (-1.0f to 1.0f)
    // This function is useful when you have two keys, left and right, and you need a single value that indicates
    // where a "joystick" would be on a horizontal axis.
    private float ConvertBooleansToAxisValue(bool negativeWeight, bool positiveWeight)
    {
        float output = 0.0f;

        if (negativeWeight)
        {
            output -= 1.0f;
        }

        if (positiveWeight)
        {
            output += 1.0f;
        }

        return output;
    }

    // Since Unity has a bit of an odd input manager design, this function is required to turn the syntactic sugar
    // enumeration used to select joystick axes from the inspector, into the hard-coded Unity input manager axis names.
    // It is not the best way to do it, but it is fairly clean and easy to use when doing things this way. Especially
    // once designers need to use the system from within the inspector window.
    private string ConstructAxisNameFromCustomAxisTypeAndJoystick(Global.Controllers joystick, Global.JoystickAxis axis)
    {
        string axisName = "";

        switch (axis)
        {
            case Global.JoystickAxis.LeftStickHorizontal:
                axisName = m_JoystickLeftStickHorizontalAxisName;
                break;

            case Global.JoystickAxis.LeftStickVertical:
                axisName = m_JoystickLeftStickVerticalAxisName;
                break;

            case Global.JoystickAxis.RightStickHorizontal:
                axisName = m_JoystickRightStickHorizontalAxisName;
                break;

            case Global.JoystickAxis.RightStackVertical:
                axisName = m_JoystickRightStickVerticalAxisName;
                break;

            case Global.JoystickAxis.TriggerLeft:
                axisName = m_JoystickLeftTriggerAxisName;
                break;

            case Global.JoystickAxis.TriggerRight:
                axisName = m_JoystickRightTriggerAxisName;
                break;

            case Global.JoystickAxis.DPadHorizontal:
                axisName = m_JoystickDPadHorizontalAxisName;
                break;

            case Global.JoystickAxis.DPadVertical:
                axisName = m_JoystickDPadVerticalAxisName;
                break;

            default:
                break;
        }

        return (axisName + (int)joystick);
    }
}
