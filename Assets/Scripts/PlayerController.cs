using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // --------------------------------------------------------------

    // --------------------------------------------------------------

    // Car suspension system
    CarSuspension m_SuspensionController;

    // The starting position of the player
    Vector3 m_SpawningPosition = Vector3.zero;

    // Whether the player is alive or not
    bool m_IsAlive = true;

    // The time it takes to respawn
    const float MAX_RESPAWN_TIME = 1.0f;
    float m_RespawnTime = MAX_RESPAWN_TIME;

    // Player ID
    Global.Player m_PlayerID;

    // Holds the input for this frame
    private float m_DriveInput;
    private float m_SteerInput;

    // --------------------------------------------------------------

    public void SetPlayerID(Global.Player player)
    {
        m_PlayerID = Global.Player.PlayerOne;
    }

    // --------------------------------------------------------------

    void Awake()
    {
        m_SuspensionController = GetComponent<CarSuspension>();
    }

    void Start()
    {
        m_SpawningPosition = transform.position;
    }

    void Update()
    {
        // If the player is dead update the respawn timer and exit update loop
        if(!m_IsAlive)
        {
            UpdateRespawnTime();
            return;
        }

        ProcessInput();
    }

    private void FixedUpdate()
    {
        // Apply the input
        m_SuspensionController.Steer(m_SteerInput);
        m_SuspensionController.Drive(m_DriveInput);
    }

    private void ProcessInput()
    {
        // Reset the input from the previous frame
        ResetInput();

        // Retrieve the player input data from the input manager
        Global.PlayerInputData inputData = InputManager.instance.GetInputDataForPlayer(m_PlayerID);

        // No input allowed for this frame
        if (!IsInputAllowed(inputData))
            return;

        // Keyboard controls use slightly different buttons and "axes" to control the car
        if (inputData.controller == Global.Controllers.Keyboard)
            HandleKeyboardDriveInput(inputData);
        else
            HandleJoystickDriveInput(inputData);

        // Steering is the same for both control types
        HandleSteerInput(inputData);
    }
    
    private void ResetInput()
    {
        m_DriveInput = 0.0f;
        m_SteerInput = 0.0f;
    }

    private bool IsInputAllowed(Global.PlayerInputData inputData)
    {
        // No need to continue if there is not controller
        if (inputData.controller == Global.Controllers.None)
            return false;

        // Cannot control the car while it is airborne
        if (m_SuspensionController.IsAirborne())
            return false;

        // All good to go
        return true;
    }

    private void HandleKeyboardDriveInput(Global.PlayerInputData inputData)
    {
        // It is easier to use the keyboard "vertical stick" for driving
        m_DriveInput = inputData.axisLeftStickVertical;
    }

    private void HandleJoystickDriveInput(Global.PlayerInputData inputData)
    {
        // Using a joystick, it is easier to drive using the A and B buttons
        if (inputData.buttonA)
        {
            // Drive
            m_DriveInput = 1.0f;
        }
        else if (inputData.buttonB)
        {
            // Reverse
            m_DriveInput = -1.0f;
        }
    }

    private void HandleSteerInput(Global.PlayerInputData inputData)
    {
        if (m_DriveInput > 0)
        {
            // Driving forwards, use regular steering behavior
            m_SteerInput = inputData.axisLeftStickHorizontal;
        }
        else if (m_DriveInput < 0)
        {
            // Driving backwards, use inverted steering behavior
            m_SteerInput = -inputData.axisLeftStickHorizontal;
        }
    }

    public void Die()
    {
        m_IsAlive = false;
        m_RespawnTime = MAX_RESPAWN_TIME;
    }

    void UpdateRespawnTime()
    {
        m_RespawnTime -= Time.deltaTime;
        if (m_RespawnTime < 0.0f)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        m_IsAlive = true;
        transform.position = m_SpawningPosition;
        transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    }
}
