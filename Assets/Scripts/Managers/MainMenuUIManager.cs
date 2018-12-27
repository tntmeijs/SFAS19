using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Animator set-up")]
    [Header("Main menu")]
    [SerializeField] private string m_BackName = "ClickBack";
    [SerializeField] private string m_PlayName = "ClickPlay";

    [Header("Play section")]
    [SerializeField] private string m_TournamentName = "ClickTournament";

    // --------------------------------------------------------------

    private Animator m_Animator = null;

    // --------------------------------------------------------------
    
    public void ClickBack()
    {
        m_Animator.SetTrigger(m_BackName);
    }

    public void ClickedPlay()
    {
        m_Animator.SetTrigger(m_PlayName);
    }

    public void ClickedTournament()
    {
        m_Animator.SetTrigger(m_TournamentName);
    }

    // --------------------------------------------------------------

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();

        if (!m_Animator)
        {
            Debug.LogError("Animator not attached to the same object as this script!");
        }
    }
}
