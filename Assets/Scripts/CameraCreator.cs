using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCreator : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Name of the cameras to make it easy to identify in the hierarchy
    [SerializeField]
    private string m_CameraNamePrefix = "PlayerCamera";

    // Background color
    [SerializeField]
    private Color m_CameraSkyColor = new Color(0.1098f, 0.8588f, 0.8039f);

    // --------------------------------------------------------------

    // References to all cameras created by this script (player ID in ascending order starting with player one)
    private GameObject[] m_CreatedCameras = new GameObject[Global.MaximumNumberOfPlayers];
    
    // --------------------------------------------------------------

    // All possible split-screen configurations
    public enum SplitStyle
    {
        Invalid = -1,

        SingleCamera,               // Cover the complete screen using camera #1

        DoubleCameraHorizontal,     // Split the screen in two equally-sized views horizontally
        DoubleCameraVertical,       // Split the screen in two equally-sized views vertically

        TripleCameraQuad,           // Split the screen in four equally-sized views with the bottom-right tile unused
        TripleCameraTop,            // Split the screen in two equally-sized views on the top with a large view on the bottom
        TripleCameraBottom,         // Split the screen in two equally-sized views on the bottom with a large view on the top
        TripleCameraLeft,           // Split the screen in two equally-sized views on the left with a large view on the right
        TripleCameraRight,          // Split the screen in two equally-sized views on the right with a large view on the left
        TripleCameraHorizontal,     // Split the screen in three equally-sized views horizontally
        TripleCameraVertical,       // Split the screen in three equally-sized views vertically

        QuadCamera                  // Split the screen in four equally-sized views
    }

    // --------------------------------------------------------------

    // Create a split-screen set-up
    public void CreateSplitScreenSetup(SplitStyle splitStyle)
    {
        // Select the correct creation function based on the chosen split-screen style
        switch (splitStyle)
        {
            case SplitStyle.SingleCamera:
                CreateSingleSetup();
                break;

            case SplitStyle.DoubleCameraHorizontal:
                CreateDoubleHorizontalSetup();
                break;

            case SplitStyle.DoubleCameraVertical:
                CreateDoubleVerticalSetup();
                break;

            case SplitStyle.TripleCameraQuad:
                CreateTripleQuadSetup();
                break;

            case SplitStyle.TripleCameraTop:
                CreateTripleTopSetup();
                break;

            case SplitStyle.TripleCameraBottom:
                CreateTripleBottomSetup();
                break;

            case SplitStyle.TripleCameraLeft:
                CreateTripleLeftSetup();
                break;

            case SplitStyle.TripleCameraRight:
                CreateTripleRightSetup();
                break;

            case SplitStyle.TripleCameraHorizontal:
                CreateTripleHorizontalSetup();
                break;

            case SplitStyle.TripleCameraVertical:
                CreateTripleVerticalSetup();
                break;

            case SplitStyle.QuadCamera:
                CreateQuadSetup();
                break;

            default:
                // The default option is to create a single camera for player one and ignore all others
                Debug.LogWarning("Invalid split-screen configuration chosen, using default single camera configuration...");
                CreateSingleSetup();
                break;
        }
    }

    public GameObject GetCameraForPlayer(Global.Player playerID)
    {
        // Since the cameras are created below (hard-coded), we can always assume this list has the players stored in
        // ascending order based on ID.
        return m_CreatedCameras[(int)playerID];
    }

    // --------------------------------------------------------------

    private void CreateSingleSetup()
    {
        CreateCamera(Global.Player.PlayerOne, 0.0f, 0.0f, 1.0f, 1.0f);  // Full screen
    }

    private void CreateDoubleHorizontalSetup()
    {
        CreateCamera(Global.Player.PlayerOne, 0.0f, 0.5f, 1.0f, 0.5f);  // Top view
        CreateCamera(Global.Player.PlayerTwo, 0.0f, 0.0f, 1.0f, 0.5f);  // Bottom view
    }

    private void CreateDoubleVerticalSetup()
    {
        CreateCamera(Global.Player.PlayerOne, 0.0f, 0.0f, 0.5f, 1.0f);  // Left view
        CreateCamera(Global.Player.PlayerTwo, 0.5f, 0.0f, 0.5f, 1.0f);  // Right view
    }

    private void CreateTripleQuadSetup()
    {
        CreateCamera(Global.Player.PlayerOne,   0.0f, 0.5f, 0.5f, 0.5f);    // Top-left view
        CreateCamera(Global.Player.PlayerTwo,   0.5f, 0.5f, 0.5f, 0.5f);    // Top-right view
        CreateCamera(Global.Player.PlayerThree, 0.0f, 0.0f, 0.5f, 0.5f);    // Bottom-left view
        CreateCamera(Global.Player.Invalid,     0.5f, 0.0f, 0.5f, 0.5f);    // Bottom-right view
    }

    private void CreateTripleTopSetup()
    {
        CreateCamera(Global.Player.PlayerOne,   0.0f, 0.5f, 0.5f, 0.5f);    // Top-left view
        CreateCamera(Global.Player.PlayerTwo,   0.5f, 0.5f, 0.5f, 0.5f);    // Top-right view
        CreateCamera(Global.Player.PlayerThree, 0.0f, 0.0f, 1.0f, 0.5f);    // Bottom view
    }

    private void CreateTripleBottomSetup()
    {
        CreateCamera(Global.Player.PlayerOne,   0.0f, 0.5f, 1.0f, 0.5f);    // Top view
        CreateCamera(Global.Player.PlayerTwo,   0.0f, 0.0f, 0.5f, 0.5f);    // Bottom-left view
        CreateCamera(Global.Player.PlayerThree, 0.5f, 0.0f, 0.5f, 0.5f);    // Bottom-right view
    }

    private void CreateTripleLeftSetup()
    {
        CreateCamera(Global.Player.PlayerOne,   0.0f, 0.5f, 0.5f, 0.5f);    // Top-left view
        CreateCamera(Global.Player.PlayerTwo,   0.0f, 0.0f, 0.5f, 0.5f);    // Bottom-left view
        CreateCamera(Global.Player.PlayerThree, 0.5f, 0.0f, 0.5f, 1.0f);    // Left view
    }

    private void CreateTripleRightSetup()
    {
        CreateCamera(Global.Player.PlayerOne,   0.5f, 0.5f, 0.5f, 0.5f);    // Top-right view
        CreateCamera(Global.Player.PlayerTwo,   0.5f, 0.0f, 0.5f, 0.5f);    // Bottom-right view
        CreateCamera(Global.Player.PlayerThree, 0.0f, 0.0f, 0.5f, 1.0f);    // Right view
    }

    private void CreateTripleHorizontalSetup()
    {
        CreateCamera(Global.Player.PlayerOne,   0.0f, 0.66666f, 1.0f, 0.33333f);    // Top view
        CreateCamera(Global.Player.PlayerTwo,   0.0f, 0.33333f, 1.0f, 0.33333f);    // Center view
        CreateCamera(Global.Player.PlayerThree, 0.0f, 0.0f,     1.0f, 0.33333f);    // Bottom view
    }

    private void CreateTripleVerticalSetup()
    {
        CreateCamera(Global.Player.PlayerOne,   0.0f,     0.0f, 0.33333f, 1.0f);    // Left view
        CreateCamera(Global.Player.PlayerTwo,   0.33333f, 0.0f, 0.33333f, 1.0f);    // Center view
        CreateCamera(Global.Player.PlayerThree, 0.66666f, 0.0f, 0.33333f, 1.0f);    // Right view
    }

    private void CreateQuadSetup()
    {
        CreateCamera(Global.Player.PlayerOne,   0.0f, 0.5f, 0.5f, 0.5f);    // Top-left view
        CreateCamera(Global.Player.PlayerTwo,   0.5f, 0.5f, 0.5f, 0.5f);    // Top-right view
        CreateCamera(Global.Player.PlayerThree, 0.0f, 0.0f, 0.5f, 0.5f);    // Bottom-left view
        CreateCamera(Global.Player.PlayerFour,  0.5f, 0.0f, 0.5f, 0.5f);    // Bottom-right view
    }

    // --------------------------------------------------------------

    private void CreateCamera(Global.Player playerID, float x, float y, float width, float height)
    {
        // Object to hold the camera component and script
        GameObject cameraObject = new GameObject("CameraPlayer" + playerID);
        
        // Add a camera component to turn the object into a camera
        Camera newCamera = cameraObject.AddComponent<Camera>();

        // Apply camera background color
        newCamera.backgroundColor = m_CameraSkyColor;
        newCamera.clearFlags = CameraClearFlags.Color;

        // Set the camera to the specified viewport dimensions
        newCamera.rect = new Rect(x, y, width, height);

        // Save the camera for future use
        m_CreatedCameras[(int)playerID] = cameraObject;
    }
}
