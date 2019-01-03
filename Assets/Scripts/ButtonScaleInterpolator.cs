using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScaleInterpolator : MonoBehaviour
{
    // --------------------------------------------------------------

    // Scale "up and down"s per second
    [SerializeField]
    private float m_Frequency = 2.0f;

    // Maximum size at the "peak" of the interpolation
    [SerializeField]
    private float m_MaximumSizeInPercentage = 110.0f;

    // --------------------------------------------------------------

    // Scale of the button as set by the user via the UI editor
    private Vector2 m_InitialScale = new Vector2(1.0f, 1.0f);

    // Target size of the button
    private Vector2 m_FinalScale = new Vector2(1.0f, 1.0f);

    // Timer
    private float m_Accumulator = 0.0f;

    // Should the interpolation reverse?
    private bool m_ReverseInterpolation = false;

    // --------------------------------------------------------------

    private void Awake()
    {
        m_InitialScale = transform.localScale;
        m_FinalScale = m_InitialScale * (m_MaximumSizeInPercentage * 0.01f);
    }

    private void Update()
    {
        // Change the scale over time by interpolating from the initial scale to the maximum, and back
        if (m_ReverseInterpolation)
            transform.localScale = Vector3.Lerp(m_InitialScale, m_FinalScale, m_Accumulator);
        else
            transform.localScale = Vector3.Lerp(m_FinalScale, m_InitialScale, m_Accumulator);

        // Reset the timer once it hits one and reverse the interpolation "direction"
        if (m_Accumulator >= 1.0f)
        {
            m_Accumulator = 0.0f;
            m_ReverseInterpolation = !m_ReverseInterpolation;
        }

        // Update the timer
        m_Accumulator += Time.deltaTime * m_Frequency;
    }
}
