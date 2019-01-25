using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    // --------------------------------------------------------------
    
    // --------------------------------------------------------------

    private PowerupType m_CurrentPowerUp = PowerupType.None;

    // --------------------------------------------------------------

    public enum PowerupType
    {
        None = -1,

        Booster,
        Missiles,

        TotalNumberOfTypes
    }

    public void SetPowerUp(PowerupType type)
    {
        m_CurrentPowerUp = type;
    }

    // Activate the current active power-up
    public void Activate()
    {
        // Activate the proper power-up
        switch (m_CurrentPowerUp)
        {
            case PowerupType.None:
                break;

                // Boost pad power-up
            case PowerupType.Booster:
                PowerupBoost boost = new PowerupBoost();
                boost.ActivatePowerUp();
                break;

                // Missile volley power-up
            case PowerupType.Missiles:
                PowerupMissile missile = new PowerupMissile();
                missile.ActivatePowerUp();
                break;
        }

        // Power-up has been used or was not available in the first place
        m_CurrentPowerUp = PowerupType.None;
    }

    // --------------------------------------------------------------

    private void Start()
    {

    }

    private void Update()
    {

    }
}
