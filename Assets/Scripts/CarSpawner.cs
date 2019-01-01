using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Prefix added to the car index in the hierarchy --> keeps the hierarchy looking nice and makes it easy to understand
    [SerializeField]
    private string m_CarNamePrefix = "CarPlayer_";

    [Header("References")]
    // Transforms where cars should be spawned in a level
    [SerializeField]
    private Transform[] m_SpawnPoints = new Transform[Global.MaximumNumberOfPlayers];

    // Cars for the players
    [SerializeField]
    private GameObject[] m_PlayerCars = new GameObject[Global.MaximumNumberOfPlayers];

    // Since the game scenes all rely on the main menu party set-up, a debug input manager
    // is used to circumvent this issue.
#if UNITY_EDITOR
    private GameObject m_InputManagerDebug = null;
#endif

    // --------------------------------------------------------------

    private void Awake()
    {
        // Only ever used when running a single scene in the Unity editor
#if UNITY_EDITOR
        DebugInitialize();
#endif

        CheckIfReferencesAreSet();
        SpawnCars();
    }

#if UNITY_EDITOR
    private void DebugInitialize()
    {
        m_InputManagerDebug = new GameObject("Debug Input Manager");

        InputManager inputManager = m_InputManagerDebug.AddComponent<InputManager>();
        inputManager.SetControllerTypeForPlayer(Global.Player.PlayerOne, Global.Controllers.Keyboard);
        inputManager.SetControllerTypeForPlayer(Global.Player.PlayerTwo, Global.Controllers.Joystick1);
    }
#endif

    private void CheckIfReferencesAreSet()
    {
        bool failed = false;

        foreach (var point in m_SpawnPoints)
        {
            if (!point)
                failed = true;
        }

        foreach (var car in m_PlayerCars)
        {
            if (!car)
                failed = true;
        }

        if (failed)
        {
            Debug.LogError("CRITICAL ERROR: Not all references have been set!");
        }
    }

    private void SpawnCars()
    {
        for (int playerIndex = 0; playerIndex < Global.MaximumNumberOfPlayers; ++playerIndex)
        {
            // Spawn the car
            GameObject car = Instantiate(m_PlayerCars[playerIndex], m_SpawnPoints[playerIndex].position, m_SpawnPoints[playerIndex].rotation);
            car.name = m_CarNamePrefix + (playerIndex + 1); // Easy to remember name, nothing important, but it keeps the scene hierarchy looking nice

            if (InputManager.instance.GetInputDataForPlayer((Global.Player)playerIndex).controller != Global.Controllers.None)
                car.AddComponent<PlayerController>();   // Human player needs a player controller
            else
                car.AddComponent<AIController>();       // AI player needs an AI controller
        }
    }
}
