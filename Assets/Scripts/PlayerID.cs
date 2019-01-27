using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerID : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Player ID of the game-object
    [SerializeField]
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
