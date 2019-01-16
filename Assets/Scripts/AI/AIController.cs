using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    // --------------------------------------------------------------

    // --------------------------------------------------------------

    // The current movement direction in x & z.
    Vector3 m_MovementDirection = Vector3.zero;

    // Whether the player is alive or not
    bool m_IsAlive = true;

    // --------------------------------------------------------------

    void Awake()
    {
    }

    void Start()
    {
    }

    void Update()
    {
        // If the player is dead update the respawn timer and exit update loop
        if (!m_IsAlive)
        {
            return;
        }
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    void RotateCharacter(Vector3 movementDirection)
    {
        Quaternion lookRotation = Quaternion.LookRotation(movementDirection);
        if (transform.rotation != lookRotation)
        {
            transform.rotation = lookRotation;
        }
    }

    public void Die()
    {
        m_IsAlive = false;
    }
}
