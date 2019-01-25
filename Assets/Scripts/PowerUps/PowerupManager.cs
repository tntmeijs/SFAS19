using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
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

    // --------------------------------------------------------------

    private void Start()
    {

    }

    private void Update()
    {

    }
}
