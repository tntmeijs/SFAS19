using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleDetector : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Tag associated with the ball
    [SerializeField]
    private string m_BallTag = "Ball";

    [Header("Particles")]
    // Confetti will show as soon as the ball hits the hole
    [SerializeField]
    private ParticleSystem m_Confetti;

    // --------------------------------------------------------------

    // Event that gets triggered when the ball lands in the hole
    public delegate void BallHitsHole(Global.Player playerThatScored);
    public BallHitsHole OnBallInHole;

    // --------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        // Ball hits the hole
        if (other.gameObject.tag == m_BallTag)
        {
            // Create the confetti particle effect and schedule its destruction until after it has finished playing
            Destroy(Instantiate(m_Confetti, transform.position, transform.rotation), m_Confetti.main.duration);

            // Get the player ID of the player that hit the ball last
            Global.Player lastPlayer = other.GetComponent<BallDataTracker>().GetLastPlayerThatTouchedBall();

            // Notify any listeners of the ball in hole event
            try
            {
                // Signal all listeners
                OnBallInHole(lastPlayer);
            }
            catch (NullReferenceException ex)
            {
            	// Nobody was listening for this event to happen, no big deal...
            }
        }
    }
}
