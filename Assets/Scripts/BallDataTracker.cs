using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDataTracker : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Car tag")]
    // Tag associated with the cars
    [SerializeField]
    private string m_CarTag = "Car";

    // --------------------------------------------------------------

    // Last player that managed to touch the ball
    private Global.Player m_LastPlayer;

    // --------------------------------------------------------------
    
    public Global.Player GetLastPlayerThatTouchedBall()
    {
        return m_LastPlayer;
    }

    // --------------------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == m_CarTag)
            m_LastPlayer = collision.gameObject.GetComponent<PlayerID>().GetPlayerID();
    }
}
