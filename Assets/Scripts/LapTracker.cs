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

    // Show the scoreboard for this many seconds before cutting to the main menu
    [SerializeField]
    private float m_ReturnToMainMenuTimeout = 10.0f;

    [SerializeField]
    // Delay before the main menu fade-out will be started
    private float m_MainMenuFadeOutDelay = 7.0f;

    // Name of the animation trigger that starts the fade-out to the scoreboard scene
    [SerializeField]
    private string m_FadeOutAnimationTrigger = "StartFade";

    // Name of the animation trigger that starts the fade-out to the main menu scene
    [SerializeField]
    private string m_MainMenuFadeOutTrigger = "MainMenuFade";

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

    // Text that will hold the final score
    [SerializeField]
    private Text m_FinalScoreText = null;

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

            // Check if we every racer has finished already
            CheckIfScoreboardCanBeShown();
        };
    }

    private void CheckNullReferences()
    {
        // Check if the reference has been set
        if (!m_LapCounter                   ||
            !m_PlayerLapCounters[0]         ||
            !m_PlayerLapCounters[1]         ||
            !m_PlayerLapCounters[2]         ||
            !m_PlayerLapCounters[3]         ||
            !m_WaypointContainer            ||
            !m_FadeOutLapInfoCanvasAnimator ||
            !m_FinalScoreText)
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
        if (m_LapsCompleted[playerIndex] >= m_LapsToWin)
        {
            // Find which place this player managed to finish at
            int place = m_FinishedPlayers.IndexOf(playerIndex);

            // The value should always be in the list, but just in case it is not...
            if (place == -1)
                return;

            m_PlayerLapCounters[playerIndex].text = "Player " + (playerIndex + 1) + " - FINISHED";
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
    }

    private void CheckIfScoreboardCanBeShown()
    {
        // Move on to the final scoreboard once the last player finishes
        if (m_FinishedPlayers.Count == Global.MaximumNumberOfPlayers)
        {
            // Set the scoreboard text
            for (int place = 0; place < m_FinishedPlayers.Count; ++place)
            {
                switch (place)
                {
                    case 0:
                        m_FinalScoreText.text = "Player " + (m_FinishedPlayers[place] + 1) + " came in first!";
                        break;

                    case 1:
                        m_FinalScoreText.text += "\nPlayer " + (m_FinishedPlayers[place] + 1) + " came in second.";
                        break;

                    case 2:
                        m_FinalScoreText.text += "\nPlayer " + (m_FinishedPlayers[place] + 1) + " came in third.";
                        break;

                    case 3:
                        m_FinalScoreText.text += "\nPlayer " + (m_FinishedPlayers[place] + 1) + " came in last... :(";
                        break;

                    default:
                        break;
                }
            }

            StartCoroutine(ViewScoreboard());
        }
    }

    private IEnumerator ViewScoreboard()
    {
        yield return new WaitForSeconds(m_ScoreboardShowTimeout);

        // Start the animation to make everything black
        m_FadeOutLapInfoCanvasAnimator.SetTrigger(m_FadeOutAnimationTrigger);

        // Prepare to go to the main menu again
        StartCoroutine(LoadMainMenu());

        // Start the fade-out to the main menu
        StartCoroutine(StartMainMenuFadeWithDelay());
    }

    private IEnumerator StartMainMenuFadeWithDelay()
    {
        yield return new WaitForSeconds(m_MainMenuFadeOutDelay);

        m_FadeOutLapInfoCanvasAnimator.SetTrigger(m_MainMenuFadeOutTrigger);
    }

    private IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(m_ReturnToMainMenuTimeout);

        // Go to the main menu (assume it is always the first scene in the build order)
        SceneManager.LoadScene(0);
    }
}
