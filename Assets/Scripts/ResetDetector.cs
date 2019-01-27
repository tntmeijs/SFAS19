using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetDetector : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Time between checks in seconds whether the car is no longer moving correctly
    [SerializeField]
    private float m_CheckDelay = 5.0f;

    // Offset in the air when resetting the car (avoids clipping through the ground)
    [SerializeField]
    private float m_ResetHeight = 2.0f;

    // --------------------------------------------------------------

    // Object that contains the ideal racing line way points
    private Transform m_WaypointContainer = null;

    // --------------------------------------------------------------

    public void SetWaypointContainer(Transform container)
    {
        m_WaypointContainer = container;
    }

    private void Start()
    {
        StartCoroutine(ResetCarLoop());
    }

    private IEnumerator ResetCarLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_CheckDelay);

            // If the car is in some kind of trouble, reset its position and orientation
            if (CarShouldBeReset())
                ResetVehicle();
        }
    }

    private bool CarShouldBeReset()
    {
        // Car is slightly angled towards the ground, assume it is upside-down
        return Vector3.Dot(transform.up, Vector3.down) > 0.1f;
    }

    private void ResetVehicle()
    {
        // Find the closest node in the way point system
        AIController aiController = GetComponent<AIController>();

        if (!aiController)
        {
            // By default, the closest node is the start node
            int closestNodeIndex = 0;

            // Closest distance to a node
            float closestDistanceFoundToNode = -1.0f;

            // This is a player, so we just place the car at the nearest track node
            for (int i = 0; i < m_WaypointContainer.childCount; ++i)
            {
                float distanceToNode = Vector3.Distance(m_WaypointContainer.GetChild(i).position, transform.position);

                // If this is a shorter distance or the distance has never been set yet, record this distance
                if (distanceToNode < closestDistanceFoundToNode ||
                    closestDistanceFoundToNode == -1.0f)
                {
                    closestNodeIndex = i;
                    closestDistanceFoundToNode = distanceToNode;
                }
            }

            // Place the car back on track (sligtly in the air to avoid clipping through the ground)
            transform.position = m_WaypointContainer.GetChild(closestNodeIndex).position + (Vector3.up * m_ResetHeight);
        }
        else
        {
            // This is an AI player, so use the node data in the AI controller to place it back on track
            transform.position = aiController.GetCurrentWaypoint().position;
        }

        // Save the old forward vector
        Vector3 oldForward = transform.forward;

        // Reset the rotation
        transform.rotation = Quaternion.identity;

        // Restore the orientation before the rotation reset occurred
        transform.forward = oldForward;
    }
}
