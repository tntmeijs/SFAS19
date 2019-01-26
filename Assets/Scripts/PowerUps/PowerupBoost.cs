using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarSuspension))]
public class PowerupBoost : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // The boost speed is added to the current speed
    [SerializeField]
    float m_BoostSpeed = 1.0f;

    // This is how long the boost will last (seconds in game time, slow-motion influences this)
    [SerializeField]
    float m_BoostTime = 1.0f;

    // --------------------------------------------------------------

    // Flag used to make the fixed update loop start applying the boost
    bool m_IsBoosting = false;
    
    // The script that drives the hover car
    CarSuspension m_CarSuspension = null;

    // --------------------------------------------------------------

    private void Awake()
    {
        // Save a reference to the current car suspension component for future use in the fixed update loop
        m_CarSuspension = GetComponent<CarSuspension>();

        // Only enable the power-up when the power-up manager activates it
        enabled = false;
    }

    private void OnEnable()
    {
        // Start boosting
        StartCoroutine(ApplyBoostOverTime());
    }

    // Reset the state of the power-up
    private void OnDisable()
    {
        m_IsBoosting = false;
    }

    private IEnumerator ApplyBoostOverTime()
    {
        // Start boosting the player
        m_IsBoosting = true;

        yield return new WaitForSeconds(m_BoostTime);
        
        // The boost is over, stop boosting until the component is enabled again (just disable the component)
        enabled = false;
    }

    private void FixedUpdate()
    {
        m_CarSuspension.Drive(m_BoostSpeed);
    }
}
