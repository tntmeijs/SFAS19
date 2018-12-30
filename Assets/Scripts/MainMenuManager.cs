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
    // Cursor icon
    [SerializeField]
    private Sprite m_CursorIcon;

    // Cursor colors
    [SerializeField]
    private Color[] m_PlayerCursorColors = new Color[Global.MAXIMUM_NUMBER_OF_PLAYERS];

    // Cursor icon dimensions
    [SerializeField]
    private Vector2 m_CursorSize = new Vector2(100.0f, 100.0f);

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

    private void EnableCursor(Global.Player player)
    {
        m_PlayerHasCursor[(int)player] = true;
        m_PlayerCursors[(int)player].SetActive(true);
    }

    private void DisableCursor(Global.Player player)
    {
        m_PlayerHasCursor[(int)player] = false;
        m_PlayerCursors[(int)player].SetActive(false);
    }
}
