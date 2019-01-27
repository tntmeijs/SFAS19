using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LapTracker : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Object that tracks the laps
    [SerializeField]
    private LapCounter m_LapCounter;

    // Number of laps to win the race
    [SerializeField]
    [Range(1, 10)]
    private int m_LapsToWin = 3;

    // Color of the win text
    [SerializeField]
    private Color m_WinTextColor = Color.green;

    // Show the scoreboard after this many seconds
    [SerializeField]
    private float m_ScoreboardShowTimeout = 3.0f;

    // Name of the animation trigger that starts the fade-out to the scoreboard scene
    [SerializeField]
    private string m_FadeOutAnimationTrigger = "StartFade";

    [Header("References")]
    // Way point container holding the ideal racing line
    [SerializeField]
    private Transform m_WaypointContainer = null;

    // Canvas that holds the lap information UI as well as the fade-out animation
    [SerializeField]
    private Animator m_FadeOutLapInfoCanvasAnimator = null;

    // Text that holds the lap information UI for player one to four
    [SerializeField]
    private Text[] m_PlayerLapCounters = new Text[Global.MaximumNumberOfPlayers];

    // --------------------------------------------------------------

    // Keep track of the number of laps per player
    private int[] m_LapsCompleted = new int[Global.MaximumNumberOfPlayers];

    // Order in which the players finished
    private List<int> m_FinishedPlayers = new List<int>();

    // --------------------------------------------------------------

    private void Awake()
    {
        CheckNullReferences();
        SetInitialCounterValues();

        // Increment the lap counter and update the UI accordingly when the player finishes a lap
        m_LapCounter.OnCarFinishesLap += (playerID, car) => {
            ++m_LapsCompleted[(int)playerID];
            
            // The required number of laps have been completed by this player
            if (m_LapsCompleted[(int)playerID] == m_LapsToWin)
                PlayerFinished((int)playerID, car);

            // Update the scoreboard text
            UpdateLapVisualization((int)playerID);
        };
    }

    private void CheckNullReferences()
    {
        // Check if the reference has been set
        if (!m_LapCounter           ||
            !m_PlayerLapCounters[0] ||
            !m_PlayerLapCounters[1] ||
            !m_PlayerLapCounters[2] ||
            !m_PlayerLapCounters[3] ||
            !m_WaypointContainer    ||
            !m_FadeOutLapInfoCanvasAnimator)
            Debug.LogError("Error: not all references have been set correctly.");
    }

    private void SetInitialCounterValues()
    {
        // Since the players will trigger the lap counter as soon as the race stars, we have to compensate for it by
        // initializing the lap counters to -1 instead of 0.
        for (int i = 0; i < Global.MaximumNumberOfPlayers; ++i)
            m_LapsCompleted[i] = -1;
    }

    private void UpdateLapVisualization(int playerIndex)
    {
        // Player finished all laps already
        if (m_LapsCompleted[playerIndex] == m_LapsToWin)
        {
            // Find which place this player managed to finish at
            int place = m_FinishedPlayers.IndexOf(playerIndex);

            // The value should always be in the list, but just in case it is not...
            if (place == -1)
                return;

            m_PlayerLapCounters[playerIndex].text = "Player " + playerIndex + " placed #" + (place + 1) + "/" + Global.MaximumNumberOfPlayers;
            m_PlayerLapCounters[playerIndex].color = m_WinTextColor;
        }
        else
        {
            // Update the player text with the latest lap information
            m_PlayerLapCounters[playerIndex].text = "Player " + (playerIndex + 1) + ": " + m_LapsCompleted[playerIndex] + "/" + m_LapsToWin + " laps";
        }
    }

    private void PlayerFinished(int id, GameObject car)
    {
        var playerController = car.GetComponent<PlayerController>();

        // Race has finished for this player, if it is a human, take over the car control
        if (playerController)
        {
            // No more control using the player input
            Destroy(playerController);

            // Add AI control (make AI aware of the way points)
            car.AddComponent<AIController>().SetWaypointContainer(m_WaypointContainer);
        }

        // Save the player in the scoreboard
        m_FinishedPlayers.Add(id);

        // Move on to the final scoreboard once the last player finishes
        if (m_FinishedPlayers.Count == Global.MaximumNumberOfPlayers)
            StartCoroutine(ViewScoreboard());
    }

    private IEnumerator ViewScoreboard()
    {
        yield return new WaitForSeconds(m_ScoreboardShowTimeout);

        // Start the animation to make everything black
        m_FadeOutLapInfoCanvasAnimator.SetTrigger(m_FadeOutAnimationTrigger);

        // Do the actual scoreboard scene loading
        StartCoroutine(LoadScoreboard());
    }

    private IEnumerator LoadScoreboard()
    {
        // Wait until the fade animation has finished fading out before switching to the scoreboard scene
        yield return new WaitForSeconds(m_FadeOutLapInfoCanvasAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        // The scoreboard is the next scene in the build order
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
