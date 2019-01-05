using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerID : MonoBehaviour
{
    // --------------------------------------------------------------

    // Player ID of the parent game-object
    private Global.Player m_PlayerID;

    // --------------------------------------------------------------

    public void SetPlayerID(Global.Player ID)
    {
        m_PlayerID = ID;
    }

    public Global.Player GetPlayerID()
    {
        return m_PlayerID;
    }
}
