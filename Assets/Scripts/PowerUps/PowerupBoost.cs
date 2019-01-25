using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupBoost : PowerUpBase
{
    // --------------------------------------------------------------

    // Flag used to make the fixed update loop start applying the boost
    bool m_IsBoosting = false;

    // The boost speed is added to the current speed
    float m_BoostSpeed = 1.0f;

    // This is how long the boost will last (seconds in game time, slow-motion influences this)
    float m_BoostTime = 1.0f;

    // The script that drives the hover car
    CarSuspension m_CarSuspension = null;

    // --------------------------------------------------------------

    public override void ActivatePowerUp()
    {
        // Save a reference to the current car suspension component for future use in the fixed update loop
        if (!m_CarSuspension)
            m_CarSuspension = gameObject.GetComponent<CarSuspension>();

        // Start boosting
        StartCoroutine(ApplyBoostOverTime());
    }

    private IEnumerator ApplyBoostOverTime()
    {
        // Start boosting the player
        m_IsBoosting = true;

        yield return new WaitForSeconds(m_BoostTime);

        // Stop boosting the player
        m_IsBoosting = false;

        // Remove this power-up component from the car
        Destroy(this);
    }

    private void FixedUpdate()
    {
        m_CarSuspension.Drive(m_BoostSpeed);
    }
}
