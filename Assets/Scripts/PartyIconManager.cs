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

    [Header("Image containers")]
    // Game-object that holds the image component for AI
    [SerializeField]
    private GameObject m_ComputerPlayerIcon;

    [SerializeField]
    // Game-object that holds the image component for humans
    private GameObject m_HumanPlayerIcon;

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
    }

    private void CheckIfReferencesAreSet()
    {
        if (!m_ComputerPlayerIcon   ||
            !m_HumanPlayerIcon      ||
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
            SetPlayerAsHuman();
    }

    private void PlayerLeftParty(int playerID)
    {
        // If the player that left is represented by the object this script is attached to, show the computer icon
        if (playerID == (int)m_PlayerNumber)
            SetPlayerAsAI();
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
}
