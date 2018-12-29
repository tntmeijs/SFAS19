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

    // Color of the join button
    [SerializeField]
    private Color m_JoinIconColor = Color.green;

    // Color of the leave button
    [SerializeField]
    private Color m_LeaveIconColor = Color.red;

    [Header("Sprites")]
    // AI icon
    [SerializeField]
    private Sprite m_ComputerIcon;

    // Player icon
    [SerializeField]
    private Sprite m_HumanIcon;

    // Join icon
    [SerializeField]
    private Sprite m_JoinIcon;

    // Leave icon
    [SerializeField]
    private Sprite m_LeaveIcon;

    [Header("Icon containers")]
    // Image that holds the player icon
    [SerializeField]
    private Image m_ProfileImage;
    
    [Header("Join / leave icons")]
    // Image that holds the button icons
    [SerializeField]
    private Image m_JoinLeaveImage;

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
        if (!m_ProfileImage     ||
            !m_JoinLeaveImage   ||
            !m_ComputerIcon     ||
            !m_HumanIcon        ||
            !m_JoinIcon         ||
            !m_LeaveIcon        ||
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
        m_ProfileImage.sprite = m_ComputerIcon;
    }

    private void SetPlayerAsHuman()
    {
        // Show the human icon
        m_ProfileImage.sprite = m_HumanIcon;
    }

    private void SetButtonJoin()
    {
        // Show the join button
        m_JoinLeaveImage.sprite = m_JoinIcon;
        m_JoinLeaveImage.color = m_JoinIconColor;
    }

    private void SetButtonLeave()
    {
        // Show the leave button
        m_JoinLeaveImage.sprite = m_LeaveIcon;
        m_JoinLeaveImage.color = m_LeaveIconColor;
    }
}
