using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The purpose of this class is to control the flow of the main menu. While the party manager and the party icon manager
/// both control the main menu as well, this class actually control the flow of the menu. For example, loading the next
/// scene, displaying options, switching between overlays, etc.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    // --------------------------------------------------------------

    // --------------------------------------------------------------

    // The input manager that lives on its own object
    private InputManager m_InputManager = null;

    // --------------------------------------------------------------

    private void Start()
    {
        // The input manager initializes on awake, which means that this call should always return a valid instance
        m_InputManager = InputManager.instance;
    }

    private void Update()
    {
        // Input manager usage sample:
        Global.PlayerInputData inputData = m_InputManager.GetInputDataForPlayer(Global.Player.PlayerOne);

        Debug.Log("Throttle: " + inputData.throttle);
        Debug.Log("Brake: " + inputData.braking);
        Debug.Log("Activate: " + inputData.activatedPowerUp);
        Debug.Log("Steering: " + inputData.steeringValue + "\n");
    }
}
