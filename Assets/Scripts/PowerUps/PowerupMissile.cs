using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PowerupMissile : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("References")]
    // Missile prefab
    [SerializeField]
    private GameObject m_MissilePrefab = null;

    [SerializeField]
    private Transform m_MissileSpawnPoint = null;

    [Header("Configuration")]
    // Random angle offset applied to the missiles
    [SerializeField]
    [Range(-15.0f, 0.0f)]
    private float m_MinRandomAngle = -5.0f;

    [SerializeField]
    [Range(0.0f, 15.0f)]
    private float m_MaxRandomAngle = 5.0f;

    // Number of missiles to launch when activating the power-up
    [SerializeField]
    [Range(1, 10)]
    private int m_MissilesPerVolley = 3;

    // Delay between missile launches
    [SerializeField]
    private float m_LaunchDelay = 0.1f;

    // --------------------------------------------------------------

    // Number of missiles launched by this component
    private int m_MissilesLaunched = 0;

    // --------------------------------------------------------------

    private void Awake()
    {
        // Check if all references have been set properly
        if (!m_MissilePrefab ||
            !m_MissileSpawnPoint)
        {
            Debug.LogError("Error: not all references have been set properly!");
        }

        // Seed the random number generator using the time since the UNIX epoch
        System.TimeSpan timeSinceUnixEpoch = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1));
        Random.InitState(timeSinceUnixEpoch.Seconds);

        // Only enable the power-up when the power-up manager activates it
        enabled = false;
    }

    // Power-ups are activated by enabling them, so every time the power-up is enabled, it should fire missiles
    private void OnEnable()
    {
        StartCoroutine(LaunchMissiles());
    }

    // Reset the state of the power-up
    private void OnDisable()
    {
        m_MissilesLaunched = 0;
    }

    // Launch missiles with a slight delay
    private IEnumerator LaunchMissiles()
    {
        // Keep launching missiles until the maximum number of missiles per volley has been reached
        do 
        {
            var missile = Instantiate(m_MissilePrefab, m_MissileSpawnPoint.position, m_MissileSpawnPoint.rotation);

            // Align the missile's forward direction with the spawn point forward direction
            missile.transform.forward = m_MissileSpawnPoint.forward;

            // Add a random offset to the missile to give it a less scripted look
            missile.transform.Rotate(Vector3.up * Random.Range(m_MinRandomAngle, m_MaxRandomAngle), Space.Self);

            // To prevent collisions with the car, the missile will inherit the car's velocity
            missile.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;

            ++m_MissilesLaunched;

            yield return new WaitForSeconds(m_LaunchDelay);
        }
        while (m_MissilesLaunched < m_MissilesPerVolley);

        // Stop launching missiles
        enabled = false;
    }
}
