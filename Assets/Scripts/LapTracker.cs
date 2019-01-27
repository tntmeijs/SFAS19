using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapTracker : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Object that tracks the laps
    [SerializeField]
    private LapCounter m_LapCounter;

    // --------------------------------------------------------------

    // Keep track of the number of laps per player
    private int[] m_LapsCompleted = new int[Global.MaximumNumberOfPlayers];    

    // --------------------------------------------------------------

    private void Awake()
    {
        CheckNullReferences();
        SetInitialCounterValues();

        // Increment the lap counter and update the UI accordingly when the player finishes a lap
        m_LapCounter.OnCarFinishesLap += playerID => {
            ++m_LapsCompleted[(int)playerID];
            UpdateLapVisualization();
        };
    }

    private void CheckNullReferences()
    {
        // Check if the reference has been set
        if (!m_LapCounter)
            Debug.LogError("Error: lap counter object not set.");
    }

    private void SetInitialCounterValues()
    {
        // Since the players will trigger the lap counter as soon as the race stars, we have to compensate for it by
        // initializaing the lap counters to -1 instead of 0.
        for (int i = 0; i < Global.MaximumNumberOfPlayers; ++i)
            m_LapsCompleted[i] = -1;
    }

    private void UpdateLapVisualization()
    {
    }
}
