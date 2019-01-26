using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Time between power-up spawns
    [SerializeField]
    private float m_PowerUpSpawnTimeOut = 5.0f;

    [Header("References")]
    // List of power-up prefabs that can be collected by the player
    [SerializeField]
    private List<GameObject> m_PowerUpVisualizations;

#if UNITY_EDITOR
    [Header("Gizmo configuration")]
    // Color of the sphere spawned at the power-up spawn point
    [SerializeField]
    private Color m_GizmoSpawnPointColor = Color.green;

    // Size of the gizmo spheres
    [SerializeField]
    private float m_GizmoSpawnPointSize = 1.0f;
#endif

    // --------------------------------------------------------------

    List<Transform> m_PowerUpSpawnPoints = null;

    // --------------------------------------------------------------

    private void Awake()
    {
        // Seed the random number generator using the time since the UNIX epoch
        System.TimeSpan timeSinceUnixEpoch = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1));
        Random.InitState(timeSinceUnixEpoch.Seconds);

        // Allocate enough storage space for all the spawn point transforms
        m_PowerUpSpawnPoints = new List<Transform>(transform.childCount);

        // Save the children for future use
        for (int i = 0; i < transform.childCount; ++i)
            m_PowerUpSpawnPoints.Add(transform.GetChild(i));

        // No need to re-spawn power-ups instantly, a coroutine is fits our needs perfectly
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_PowerUpSpawnTimeOut);
            SpawnRandomPowerUps();
        }
    }

    private void SpawnRandomPowerUps()
    {
        foreach (var child in m_PowerUpSpawnPoints)
        {
            // Only spawn a new random power-up if the child does not have a power-up yet
            if (child.childCount != 0)
                continue;

            // Create a random power-up
            Instantiate(m_PowerUpVisualizations[Random.Range(0, m_PowerUpVisualizations.Count)], child);
        }
    }

    // Only visualize the spawn points in the editor when the object is selected
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = m_GizmoSpawnPointColor;
        
        for (int i = 0; i < transform.childCount; ++i)
        {
            Gizmos.DrawSphere(transform.GetChild(i).position, m_GizmoSpawnPointSize);
        }
    }
#endif
}
