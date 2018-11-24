using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for throwing snowballs.
/// Based on the amount of snow gathered, certain snowballs can be thrown.
/// </summary>
public class ThrowLogic : MonoBehaviour
{
    // --------------------------------------------------------------
    
    [Header("Configuration")]
    [SerializeField] private float m_ThrowCooldown = 0.5f;

    [Header("References")]
    // This reference is needed to assign the snow mesh to the snowball (for the snow track rendering)
    [SerializeField] private GameObject m_SnowMesh = null;
    [SerializeField] private GameObject m_SnowballPrefab = null;

    [SerializeField] private Transform m_SnowballSpawnPoint = null;

    [SerializeField] private InputManager m_InputManager = null;

    // --------------------------------------------------------------

    // Snow gathered by grabbing it from piles of snow
    private float m_SnowAmount = 0.0f;

    // Determines whether the player is allowed to throw a snowball
    private bool m_CanThrow = true;

    // --------------------------------------------------------------

    public void AddSnow(int snowAmount)
    {
        m_SnowAmount += snowAmount;
    }

    // --------------------------------------------------------------

    private void Awake()
    {
        CheckReferencesForNull();
    }

    private void Start()
    {
        m_InputManager.OnPlayerThrowInput += InputThrow;
    }

    private void CheckReferencesForNull()
    {
        if (!m_SnowballPrefab       ||
            !m_SnowballSpawnPoint   ||
            !m_SnowMesh             ||
            !m_InputManager)
        {
            Debug.LogError("ERROR: One or more references in the throw logic script have not been set properly!");
        }
    }

    private void InputThrow()
    {
        if (m_CanThrow && m_SnowAmount > 0.0f)
        {
            ThrowSnowBall();
            m_CanThrow = false;

            StartCoroutine(ApplyThrowCooldown());
        }
    }

    private void ThrowSnowBall()
    {
        // Create the snowball from the snowball prefab
        GameObject snowball = Instantiate(m_SnowballPrefab, m_SnowballSpawnPoint.position, transform.rotation * m_SnowballPrefab.transform.rotation);

        // This reference has to be set to make the snow track rendering work
        snowball.GetComponent<DrawSnowTracksToSplatmap>().SetSnowMesh(m_SnowMesh);
    }

    private IEnumerator ApplyThrowCooldown()
    {
        yield return new WaitForSeconds(m_ThrowCooldown);
        m_CanThrow = true;
    }
}
