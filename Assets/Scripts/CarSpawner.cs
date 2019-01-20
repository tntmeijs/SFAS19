using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[RequireComponent(typeof(CameraCreator))]
public class CarSpawner : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Prefix added to the car index in the hierarchy --> keeps the hierarchy looking nice and makes it easy to understand
    [SerializeField]
    private string m_CarNamePrefix = "CarPlayer_";

    [Header("References")]
    // State machine controller that defines AI behavior
    [SerializeField]
    private AnimatorController m_AIStateMachineController;

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

        if (!m_AIStateMachineController)
            failed = true;

        if (failed)
        {
            Debug.LogError("CRITICAL ERROR: Not all references have been set!");
        }
    }

    private void SpawnCars()
    {
        // The script responsible for creating the cameras
        CameraCreator cameraCreator = GetComponent<CameraCreator>();

        // TODO: Make this configurable!
        // Create the split-screen camera set-up
        cameraCreator.CreateSplitScreenSetup(CameraCreator.SplitStyle.SingleCamera);

        for (int playerIndex = 0; playerIndex < Global.MaximumNumberOfPlayers; ++playerIndex)
        {
            // Spawn the car
            GameObject car = Instantiate(m_PlayerCars[playerIndex], m_SpawnPoints[playerIndex].position, m_SpawnPoints[playerIndex].rotation);

            // Easy to remember name, nothing important, but it keeps the scene hierarchy looking nice
            car.name = m_CarNamePrefix + (playerIndex + 1);

            if (InputManager.instance.GetInputDataForPlayer((Global.Player)playerIndex).controller != Global.Controllers.None)
            {
                // Human player needs a player controller
                PlayerController playerController = car.AddComponent<PlayerController>();

                // Set the correct player ID
                playerController.SetPlayerID((Global.Player)playerIndex);

                // Make the camera follow this player (get the camera for this player, get the camera's follow script, use the follow script to set the target)
                cameraCreator.GetCameraForPlayer((Global.Player)playerIndex).GetComponent<CameraFollow>().SetTargetTransform(car.transform);
            }
            else
            {
                // AI player needs an AI controller
                car.AddComponent<AIController>();

                // AI players are controlled by state machines, add an Animator as the state machine controller
                var animator = car.AddComponent<Animator>();
                animator.runtimeAnimatorController = m_AIStateMachineController;
            }
        }
    }
}
