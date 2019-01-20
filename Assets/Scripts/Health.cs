using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // The total health of this unit
    [SerializeField]
    private int m_Health = 100;
    
    private int m_InitialHealth = 0;

    private void Awake()
    {
        // Save the inital health value for future use
        m_InitialHealth = m_Health;
    }

    public void DoDamage(int damage)
    {
        m_Health -= damage;

        if(m_Health < 0)
        {
            Destroy(gameObject);
        }
    }

    public int GetMaximumHealthValue()
    {
        return m_InitialHealth;
    }

    public int GetCurrentHealthValue()
    {
        return m_Health;
    }

    public bool IsAlive()
    {
        return m_Health > 0;
    }
}
