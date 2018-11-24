using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The order of this enumeration matters, it has to match the joystick names
// in the Unity editor input settings. There does not seem to be a proper way
// to circumvent the Unity input settings panel to get reliable joystick input
// values. Therefore, this "hack" is used. Sorry.
public enum JoystickAxis
{
    Invalid = -1,
    
    Joystick_0_L,
    Joystick_0_R,

    MaxNumerOfJoysticks
};

public class InputManager : MonoBehaviour
{
    // --------------------------------------------------------------

    [SerializeField] private KeyCode m_PlayerForwardKeyCode     = KeyCode.W;
    [SerializeField] private KeyCode m_PlayerBackwardKeyCode    = KeyCode.S;
    [SerializeField] private KeyCode m_PlayerLeftKeyCode        = KeyCode.A;
    [SerializeField] private KeyCode m_PlayerRightKeyCode       = KeyCode.D;

    [SerializeField] private KeyCode m_PlayerDashKeyCode        = KeyCode.LeftShift;
    [SerializeField] private KeyCode m_PlayerJumpKeyCode        = KeyCode.Space;
    [SerializeField] private KeyCode m_PlayerThrowKeyCode       = KeyCode.Mouse0;

    // This is prefix that should be used for every joystick that is added via the
    // editor input settings, e.g.: Joystick_Axis_0, Joystick_Axis_1, etc.
    // The code below automatically loops through all joystick axes, so the
    // number will be appended automatically, this variable only needs the prefix.
    [SerializeField] private string m_JoystickInputPrefix = "Joystick_Axis_";

    // --------------------------------------------------------------

    private List<Vector2> m_JoystickValues = new List<Vector2>();

    // --------------------------------------------------------------

    public delegate void PlayerButtonInput();

    // --------------------------------------------------------------

    // Movement
    public PlayerButtonInput OnPlayerForwardInput;
    public PlayerButtonInput OnPlayerBackwardInput;
    public PlayerButtonInput OnPlayerLeftInput;
    public PlayerButtonInput OnPlayerRightInput;

    // Actions
    public PlayerButtonInput OnPlayerDashInput;
    public PlayerButtonInput OnPlayerJumpInput;
    public PlayerButtonInput OnPlayerThrowInput;

    // --------------------------------------------------------------

    public void SetPlayerForwardInputButton(KeyCode newKey)
    {
        m_PlayerForwardKeyCode = newKey;
    }

    public void SetPlayerBackwardInputButton(KeyCode newKey)
    {
        m_PlayerBackwardKeyCode = newKey;
    }

    public void SetPlayerLeftInputButton(KeyCode newKey)
    {
        m_PlayerLeftKeyCode = newKey;
    }

    public void SetPlayerRightInputButton(KeyCode newKey)
    {
        m_PlayerRightKeyCode = newKey;
    }

    public void SetPlayerJumpInputButton(KeyCode newKey)
    {
        m_PlayerJumpKeyCode = newKey;
    }

    public void SetPlayerDashInputButton(KeyCode newKey)
    {
        m_PlayerDashKeyCode = newKey;
    }

    public void SetPlayerThrowInputButton(KeyCode newKey)
    {
        m_PlayerThrowKeyCode = newKey;
    }

    public Vector2 GetJoystickValue(JoystickAxis axis)
    {
        return m_JoystickValues[(int)axis];
    }

    // --------------------------------------------------------------

    private void Awake()
    {
        // These values will be used to store the values of the joysticks connected
        // The number of joysticks has to be doubled to account for both the horizontal
        // as well as the vertical axes.
        for (ushort index = 0; index < (int)JoystickAxis.MaxNumerOfJoysticks * 2; ++index)
        {
            m_JoystickValues.Add(new Vector2());
            m_JoystickValues[index] = Vector2.zero;
        }
    }

    private void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        PollPlayerInput();
        UpdateJoystickValues();
    }

    private void PollPlayerInput()
    {
        if (!IsAnyInputDetected())
        {
            return;
        }

        // Movement
        HandleInput(m_PlayerForwardKeyCode, OnPlayerForwardInput);
        HandleInput(m_PlayerBackwardKeyCode, OnPlayerBackwardInput);
        HandleInput(m_PlayerLeftKeyCode, OnPlayerLeftInput);
        HandleInput(m_PlayerRightKeyCode, OnPlayerRightInput);

        // Actions
        HandleInput(m_PlayerDashKeyCode, OnPlayerDashInput);
        HandleInput(m_PlayerJumpKeyCode, OnPlayerJumpInput);
        HandleInput(m_PlayerThrowKeyCode, OnPlayerThrowInput);
    }

    private bool IsAnyInputDetected()
    {
        return Input.anyKey;
    }

    private void HandleInput(KeyCode keyCode, PlayerButtonInput inputCallback)
    {
        if (Input.GetKey(keyCode))
        {
            inputCallback();
        }
    }

    private void UpdateJoystickValues()
    {
        // At least once controller needs to be attached
        if (IsAnyJoystickPluggedIn())
        {
            // Please check the comment above the "m_JoystickInputPrefix" member variable to get
            // an idea of why this loop is set-up the way it is.
            // Loop through all controllers and update the stored values
            // Once again, the number of joysticks has to be doubled to account for both the horizontal
            // as well as the vertical axes.
            for (ushort index = 0; index < (int)JoystickAxis.MaxNumerOfJoysticks * 2; index += 2)
            {
                // Because both axes are grabbed at the same time, the index in the for-loop has to increment
                // by two on every iteration of the loop.
                float horizontalAxisValue   = Input.GetAxisRaw(m_JoystickInputPrefix + (index + 0).ToString());
                float verticalAxisValue     = Input.GetAxisRaw(m_JoystickInputPrefix + (index + 1).ToString());

                m_JoystickValues[index] = new Vector2(horizontalAxisValue, verticalAxisValue);
            }
        }
    }

    private bool IsAnyJoystickPluggedIn()
    {
        return (Input.GetJoystickNames().Length > 0);
    }
}
