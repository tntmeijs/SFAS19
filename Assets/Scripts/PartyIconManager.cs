using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyIconManager : MonoBehaviour
{
    // --------------------------------------------------------------
    
    // Fool-proof player number range
    public enum Player
    {
        PlayerOne   = 0,
        PlayerTwo   = 1,
        PlayerThree = 2,
        PlayerFour  = 3
    }

    [Header("Configuration")]
    // To which player ID does this icon belong?
    [SerializeField]
    private Player m_PlayerNumber = Player.PlayerOne;

    [Header("Player type icon")]
    // Game-object that holds the image component for AI
    [SerializeField]
    private GameObject m_ComputerPlayerIcon;

    // Game-object that holds the image component for humans
    [SerializeField]
    private GameObject m_HumanPlayerIcon;

    [Header("Join / leave icons")]
    // Button to join the party
    [SerializeField]
    private GameObject m_JoinButton;

    // Button to leave the party
    [SerializeField]
    private GameObject m_LeaveButton;

    [Header("References")]
    // Object that holds the party manager script
    [SerializeField]
    private PartyManager m_PartyManager = null;

    // --------------------------------------------------------------

    // Protect against decrementing the player ID more than once
    private bool m_FixedPlayerIDAlready = false;

    // --------------------------------------------------------------

    private void Awake()
    {
        CheckIfReferencesAreSet();
        PrepareCallbacks();
        SetPlayerAsAI();
        SetButtonJoin();
    }

    private void CheckIfReferencesAreSet()
    {
        if (!m_ComputerPlayerIcon   ||
            !m_HumanPlayerIcon      ||
            !m_JoinButton           ||
            !m_LeaveButton          ||
            !m_PartyManager)
        {
            Debug.LogError("CRITICAL ERROR: Not all references have been set!");
        }
    }

    private void PrepareCallbacks()
    {
        ControllerManager controllerManager = m_PartyManager.GetControllerManager();
        RegisterCallbacks(ref controllerManager);
    }

    private void RegisterCallbacks(ref ControllerManager controllerManager)
    {
        controllerManager.OnPlayerJoin  += PlayerJoinedParty;
        controllerManager.OnPlayerLeave += PlayerLeftParty;
    }

    private void PlayerJoinedParty(int playerID)
    {
        // If the player that joined is represented by the object this script is attached to, show the human icon
        if (playerID == (int)m_PlayerNumber)
        {
            SetPlayerAsHuman();
            SetButtonLeave();
        }
    }

    private void PlayerLeftParty(int playerID)
    {
        // If the player that left is represented by the object this script is attached to, show the computer icon
        if (playerID == (int)m_PlayerNumber)
        {
            SetPlayerAsAI();
            SetButtonJoin();
        }
    }

    private void SetPlayerAsAI()
    {
        // Show the AI icon
        m_ComputerPlayerIcon.SetActive(true);
        m_HumanPlayerIcon.SetActive(false);
    }

    private void SetPlayerAsHuman()
    {
        // Show the human icon
        m_ComputerPlayerIcon.SetActive(false);
        m_HumanPlayerIcon.SetActive(true);
    }

    private void SetButtonJoin()
    {
        // Show the join button
        m_JoinButton.SetActive(true);
        m_LeaveButton.SetActive(false);
    }

    private void SetButtonLeave()
    {
        // Show the leave button
        m_JoinButton.SetActive(false);
        m_LeaveButton.SetActive(true);
    }
}
