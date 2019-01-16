using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    // --------------------------------------------------------------

    // Whether the player is alive or not
    bool m_IsAlive = true;

    // --------------------------------------------------------------

    private void Update()
    {
        if (!m_IsAlive)
        {
            return;
        }
    }

    public void Die()
    {
        m_IsAlive = false;
    }
}
