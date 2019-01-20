using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    // --------------------------------------------------------------

    // Tag that indicates a split of the racing line after this node
    private string m_TrackSplitTag = "TrackSplit";

    // Tag that indicates the start of the pit lane after this node
    private string m_PitSplitTag = "PitSplit";

    // Choice when the track splits
    private int m_SplitChoice = -1;

    // Enter the pit lane whenever the health of the car falls below this threshold (percentage)
    private float m_PitLaneHealthThreshold = 0.2f;

    // When the car gets this close to the current waypoint, the waypoint system advanced the index by one
    float m_NextWaypointSelectionDistance = 8.0f;

    // Whether the player is alive or not
    bool m_IsAlive = true;

    // Waypoint index for the regular racing line
    int m_MainTrackWaypointIndex = 0;

    // Waypoint index for the nested waypoints (splits, pit lane, etc.)
    int m_NestedTrackWaypointIndex = 0;

    float m_SteeringValue = 0.0f;

    Transform m_WaypointContainer = null;

    CarSuspension m_CarSuspension = null;

    Health m_Health = null;

    enum TrackSplitOptions
    {
        Invalid = -1,

        Left,
        Right
    }

    // --------------------------------------------------------------

    // The car creation script adds the AI controller to cars, so to get access to the way point system, we pass the
    // game object containing the way points. This could have been done using a find function to find by tag or name, but
    // I chose to do things like this. Both are valid ways of doing things.
    public void SetWaypointContainer(Transform container)
    {
        m_WaypointContainer = container;
    }

    public void Die()
    {
        m_IsAlive = false;
    }

    // --------------------------------------------------------------

    private void Awake()
    {
        m_CarSuspension = GetComponent<CarSuspension>();
        m_Health = GetComponent<Health>();

        // Seed the random number generator using the time since the UNIX timestamp
        System.TimeSpan timeSinceUnixEpoch = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1));
        Random.InitState(timeSinceUnixEpoch.Seconds);
    }

    Vector3 targetPosition;
    private void Update()
    {
        if (!m_IsAlive)
        {
            return;
        }

        Transform thisNode = m_WaypointContainer.GetChild(m_MainTrackWaypointIndex);

        // Used when a waypoint is a child of an existing waypoint (nested track)
        Transform nestedThisNode = null;

        // By default, use the current node as the target to steer towards
        targetPosition = thisNode.position;

        if (thisNode.gameObject.CompareTag(m_TrackSplitTag))
        {
            // Randomly decide left of right
            if (m_SplitChoice == (int)TrackSplitOptions.Invalid)
                m_SplitChoice = Random.Range(0, 2);
            
            // m_SplitChoice == 0 --> left
            // m_SplitChoice == 1 --> right
            nestedThisNode = thisNode.GetChild(m_SplitChoice).GetChild(m_NestedTrackWaypointIndex);

            // Use a new target
            targetPosition = nestedThisNode.position;

            // Continue on the sub track
            if (Vector3.Distance(transform.position, targetPosition) < m_NextWaypointSelectionDistance)
                ++m_NestedTrackWaypointIndex;

            // Reached the end of the sub track
            if (m_NestedTrackWaypointIndex > thisNode.GetChild(m_SplitChoice).childCount - 1)
                ++m_MainTrackWaypointIndex;
        }
        else if (thisNode.gameObject.CompareTag(m_PitSplitTag))
        {
            // Enter the pit lane if the health value falls below the acceptable amount
            if (m_Health.GetCurrentHealthValue() < m_Health.GetMaximumHealthValue() * m_PitLaneHealthThreshold)
            {
                nestedThisNode = thisNode.GetChild(m_NestedTrackWaypointIndex);

                // Use a new target
                targetPosition = nestedThisNode.position;

                // Continue on the pit lane
                if (Vector3.Distance(transform.position, targetPosition) < m_NextWaypointSelectionDistance)
                    ++m_NestedTrackWaypointIndex;

                // Reached the end of the sub track
                if (m_NestedTrackWaypointIndex > thisNode.childCount - 1)
                    ++m_MainTrackWaypointIndex;
            }
            else
            {
                // Ignore the sub track and skip right to the next node
                ++m_MainTrackWaypointIndex;
            }
        }
        else
        {
            // No longer in a nested track, reset the values
            m_SplitChoice = (int)TrackSplitOptions.Invalid;
            m_NestedTrackWaypointIndex = 0;

            // Regular track following
            if (Vector3.Distance(transform.position, targetPosition) < m_NextWaypointSelectionDistance)
                ++m_MainTrackWaypointIndex;
        }
        
        // Loop to the beginning of the array
        if (m_MainTrackWaypointIndex > m_WaypointContainer.childCount - 1)
            m_MainTrackWaypointIndex = 0;

        // Normalized vector in local space from the car to the current node
        Vector3 localDirection = transform.InverseTransformVector((targetPosition - transform.position).normalized);

        // The x-component of the vector can be used to determine how far off the car is from the ideal racing line
        // When the x-component is closer to -1 or 1, the car will steer more to the left / right
        m_SteeringValue = localDirection.x;
    }

    private void FixedUpdate()
    {
        m_CarSuspension.Drive(0.5f);
        m_CarSuspension.Steer(m_SteeringValue);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(targetPosition, 1.0f);
    }
}
