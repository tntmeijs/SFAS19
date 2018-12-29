using System;
using System.Collections.Generic;

/// <summary>
/// This class handles the assignment and reassignment of controllers. It is not aware of any Unity-specific concepts.
/// Instead, it is basically a fancy list manager that keep track of the players in the party and which index they have
/// to use to get their input from the system.
/// </summary>
public class ControllerManager
{
    // --------------------------------------------------------------

    // Maps the controller to the player index
    // List[0] --> player 1
    // List[1] --> player 2
    // List[2] --> player 3
    // List[3] --> player 4
    private List<int> m_RegisteredControllersIndices;

    // --------------------------------------------------------------

    // A keyboard is considered a controller as well
    public enum Controllers
    {
        None = -1,

        Keyboard,   // 0
        Joystick1,  // 1
        Joystick2,  // 2
        Joystick3,  // 3
        Joystick4,  // 4
    }

    // Used to create callbacks when players join / leave the game. Also passes the player index.
    public delegate void PlayerJoinLeaveCallback(int index);

    // Player joins the party
    public PlayerJoinLeaveCallback OnPlayerJoin;

    // Player leaves the party
    public PlayerJoinLeaveCallback OnPlayerLeave;

    // --------------------------------------------------------------

    // Initialize by creating an empty list of controllers
    public void Initialize()
    {
        // Allocate enough memory for all players
        m_RegisteredControllersIndices = new List<int>(Settings.MAXIMUM_NUMBER_OF_PLAYERS);

        FillControllerSpots();
    }

    // Initialize by reusing an existing list of controllers
    public void Initialize(ref List<int> existingControllerList)
    {
        m_RegisteredControllersIndices = existingControllerList;
    }

    // Add a controller to the list
    public void AddGameController(Controllers type)
    {
        // The controller that is being added exceeds the maximum number of players allowed, no need to continue
        if (m_RegisteredControllersIndices.Count > Settings.MAXIMUM_NUMBER_OF_PLAYERS)
            return;

        for (int playerIndex = 0; playerIndex < Settings.MAXIMUM_NUMBER_OF_PLAYERS; ++playerIndex)
        {
            if (m_RegisteredControllersIndices[playerIndex] == (int)type)
            {
                // This controller exists already, no need to continue
                return;
            }
        }

        // Register the controller index
        for (int playerIndex = 0; playerIndex < Settings.MAXIMUM_NUMBER_OF_PLAYERS; ++playerIndex)
        {
            if (m_RegisteredControllersIndices[playerIndex] == (int)Controllers.None)
            {
                m_RegisteredControllersIndices[playerIndex] = (int)type;

                try
                {
                    // Signal other scripts that a new player has joined
                    OnPlayerJoin(playerIndex);
                }
                catch (NullReferenceException e)
                {
                    // If no callback has been registered, this will throw a NullReferenceException. It is no big deal if
                    // nothing has been registered as a callback, but to prevent the exception from surfacing, it is handled
                    // in here (or, more accurately, not handled).
                }

                // Only set one controller, if the others are invalid, leave them for the next controller
                break;
            }
        }
    }

    // Remove a controller from the list
    public void RemoveGameController(Controllers type)
    {
        // No more controllers left to unregister
        if (m_RegisteredControllersIndices.Count < 1)
            return;

        for (int playerIndex = 0; playerIndex < Settings.MAXIMUM_NUMBER_OF_PLAYERS; ++playerIndex)
        {
            // Found the controller that needs to be removed
            if (m_RegisteredControllersIndices[playerIndex] == (int)type)
            {
                // Set the controller to "none" to remove it
                m_RegisteredControllersIndices[playerIndex] = (int)Controllers.None;

                try
                {
                    // Signal other scripts that a player has left
                    OnPlayerLeave(playerIndex);
                }
                catch (NullReferenceException e)
                {
                    // If no callback has been registered, this will throw a NullReferenceException. It is no big deal if
                    // nothing has been registered as a callback, but to prevent the exception from surfacing, it is handled
                    // in here (or, more accurately, not handled).
                }
            }
        }
    }

    // Retrieve the list of controllers registered to the manager
    public List<int> GetControllerList()
    {
        return m_RegisteredControllersIndices;
    }

    // --------------------------------------------------------------

    // Fill the list with empty controllers
    private void FillControllerSpots()
    {
        // Set each controller slot to no controller at all
        for (int controller = 0; controller < Settings.MAXIMUM_NUMBER_OF_PLAYERS; ++controller)
        {
            m_RegisteredControllersIndices.Add((int)Controllers.None);
        }
    }
}
