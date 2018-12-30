using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The purpose of this class is to control the flow of the main menu. While the party manager and the party icon manager
/// both control the main menu as well, this class actually control the flow of the menu. For example, loading the next
/// scene, displaying options, switching between overlays, etc.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Main menu cursor container")]
    // Object that will act as the parent of the cursor objects
    [SerializeField]
    private Transform m_CursorParent = null;

    [Header("Cursors")]
    // Should the horizontal axis be inverted?
    [SerializeField]
    private bool m_InvertHorizontalAxis = false;

    // Should the vertical axis be inverted?
    [SerializeField]
    private bool m_InvertVerticalAxis = true;

    // Should the axis inversion be applied to the keyboard as well?
    [SerializeField]
    private bool m_ApplyInversionToKeyboard = false;

    // Speed of the cursor in pixels per second
    [SerializeField][Range(100.0f, 2500.0f)]
    private float m_CursorSpeed = 750.0f;

    // Cursor icon
    [SerializeField]
    private Sprite m_CursorIcon;

    // Cursor icon dimensions
    [SerializeField]
    private Vector2 m_CursorSize = new Vector2(100.0f, 100.0f);

    // Object that holds the cursor join animation
    [SerializeField]
    private GameObject m_CursorJoinAnimationPrefab = null;

    // Time in seconds after which the animation prefab is destroyed
    [SerializeField][Range(0.0f, 1.0f)]
    private float m_AnimationObjectLifetime = 0.5f;

    // Cursor colors
    [SerializeField]
    private Color[] m_PlayerCursorColors = new Color[Global.MAXIMUM_NUMBER_OF_PLAYERS];
    
    // --------------------------------------------------------------

    // The input manager singleton
    private InputManager m_InputManager = null;

    // The party manager singleton
    private PartyManager m_PartyManager = null;

    // Keeps track of the number of players in the current party
    private int m_PlayersInParty = 0;

    // Which players should have their own cursor?
    private bool[] m_PlayerHasCursor = new bool[Global.MAXIMUM_NUMBER_OF_PLAYERS];

    // Cursors
    private GameObject[] m_PlayerCursors = new GameObject[Global.MAXIMUM_NUMBER_OF_PLAYERS];

    // --------------------------------------------------------------

    private void Start()
    {
        SetSingletonReferences();
        CheckIfReferencesAreSet();
        SetJoinLeaveCallbacks();
        CreateCursors();
    }

    private void SetSingletonReferences()
    {
        // The input manager initializes on awake, which means that this call should always return a valid instance
        m_InputManager = InputManager.instance;

        // The party manager initializes on awake, which means that this call should always return a valid instance
        m_PartyManager = PartyManager.instance;
    }

    private void CheckIfReferencesAreSet()
    {
        if (!m_InputManager         ||
            !m_PartyManager         ||
            !m_CursorIcon           ||
            !m_CursorJoinAnimationPrefab  ||
            !m_CursorParent)
        {
            Debug.LogError("CRITICAL ERROR: Not all references have been set!");
        }
    }

    private void SetJoinLeaveCallbacks()
    {
        m_PartyManager.GetControllerManager().OnPlayerJoin += PlayerJoinsParty;
        m_PartyManager.GetControllerManager().OnPlayerLeave += PlayerLeavesParty;
    }

    private void PlayerJoinsParty(Global.Player player)
    {
        ++m_PlayersInParty;

        ShowCursorJoinAnimation(player);
        EnableCursor(player);
    }

    private void PlayerLeavesParty(Global.Player player)
    {
        --m_PlayersInParty;

        DisableCursor(player);
    }

    // The cursor objects are empty game objects by default
    // This function gives it an image component, sprite, and a parent object
    private void CreateCursors()
    {
        for (int i = 0; i < m_PlayerCursors.Length; ++i)
        {
            // Create the cursor object
            m_PlayerCursors[i] = new GameObject("Cursor" + i);
            
            // Used to display the cursor sprite
            Image image = m_PlayerCursors[i].AddComponent<Image>();

            // Add the cursor sprite to the image
            image.sprite = m_CursorIcon;

            // Set the correct color
            image.color = m_PlayerCursorColors[i];

            // Make sure the size is correct
            m_PlayerCursors[i].GetComponent<RectTransform>().sizeDelta = m_CursorSize;

            // The cursor should stay hidden until a player joins
            m_PlayerCursors[i].SetActive(false);

            // Parent the cursor object to the designated parent
            m_PlayerCursors[i].transform.SetParent(m_CursorParent, false);
        }
    }

    private void ShowCursorJoinAnimation(Global.Player player)
    {
        // Instantiate the object that holds the animation
        GameObject animationObject = Instantiate(m_CursorJoinAnimationPrefab);

        // Parent it to the cursor object
        animationObject.transform.SetParent(m_PlayerCursors[(int)player].transform, false);

        // Perform object destruction after a predetermined delay
        Destroy(animationObject, m_AnimationObjectLifetime);
    }

    private void EnableCursor(Global.Player player)
    {
        m_PlayerHasCursor[(int)player] = true;
        m_PlayerCursors[(int)player].SetActive(true);

        // It is possible that the player left the party and joined again, in this case, the cursor could be at any
        // other position. To keep things looking nice, reset the cursor to the origin of the UI again.
        m_PlayerCursors[(int)player].transform.localPosition = Vector2.zero;
    }

    private void DisableCursor(Global.Player player)
    {
        m_PlayerHasCursor[(int)player] = false;
        m_PlayerCursors[(int)player].SetActive(false);
    }

    private void Update()
    {
        // Only update the cursors that are currently active
        for (int playerIndex = 0; playerIndex < m_PlayerHasCursor.Length; ++playerIndex)
        {
            // The cursor is not active, no need to update it
            if (!m_PlayerHasCursor[playerIndex])
                continue;

            Vector2 cursorMoveDelta = Vector2.zero;

            // Get the input data for the player with the current ID
            Global.PlayerInputData inputData = m_InputManager.GetInputDataForPlayer((Global.Player)playerIndex);

            // Control the cursor position
            cursorMoveDelta.x = inputData.axisLeftStickHorizontal;
            cursorMoveDelta.y = inputData.axisLeftStickVertical;

            // Check if the inversion settings apply to the keyboard as well
            if (inputData.controller != Global.Controllers.Keyboard ||
                (inputData.controller == Global.Controllers.Keyboard && m_ApplyInversionToKeyboard))
            {
                // Apply horizontal inversion settings
                if (m_InvertHorizontalAxis)
                    cursorMoveDelta.x *= -1.0f;

                // Apply vertical inversion settings
                if (m_InvertVerticalAxis)
                    cursorMoveDelta.y *= -1.0f;
            }

            // Make sure each movement direction moves the cursor at the same speed
            cursorMoveDelta.Normalize();

            // Input speed should be relative to the refresh rate of the game
            cursorMoveDelta *= (m_CursorSpeed * Time.deltaTime);

            // Update the position of the cursor
           m_PlayerCursors[playerIndex].transform.localPosition += new Vector3(cursorMoveDelta.x, cursorMoveDelta.y, 0.0f);
        }
    }
}
