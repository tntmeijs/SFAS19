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

    // Tag that indicated a brake volume
    private string m_BrakeVolumeTag = "BrakeZone";

    // Choice when the track splits
    private int m_SplitChoice = -1;

    // way point index for the regular racing line
    private int m_MainTrackWaypointIndex = 0;

    // way point index for the nested way points (splits, pit lane, etc.)
    private int m_NestedTrackWaypointIndex = 0;

    // Make the AI feel more human by giving it a random error margin
    // It is important to keep these numbers close to 1, because the throttle and steering values will be multiplied
    // by a random value between the minimum and the maximum.
    private float m_MinimumErrorMargin = 0.9f;
    private float m_MaximumErrorMargin = 1.1f;

    // Enter the pit lane whenever the health of the car falls below this threshold (percentage)
    private float m_PitLaneHealthThreshold = 0.2f;

    // When the car gets this close to the current way point, the way point system advanced the index by one
    private float m_NextWaypointSelectionDistance = 8.0f;

    // Maximum throttle when the car AI can drive straight ahead
    private float m_FullThrottle = 1.0f;

    // Braking throttle when the car is in a brake trigger volume
    private float m_BrakeThrottle = 0.1f;

    // Steering value passed to the suspension script (between -1 and 1)
    private float m_SteeringValue = 0.0f;

    // Whether the player is alive or not
    private bool m_IsAlive = true;

    // Object that contains the ideal racing line way points
    private Transform m_WaypointContainer = null;

    // Script that allows the car to drive
    private CarSuspension m_CarSuspension = null;

    // Health of the player
    private Health m_Health = null;
    
    // More human-readble version of a 0 == false and 1 == true choice down below
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
        // Set references
        // It is safe to assume that these scripts exist on this game object, as it is part of its prefab
        m_CarSuspension = GetComponent<CarSuspension>();
        m_Health = GetComponent<Health>();

        // Seed the random number generator using the time since the UNIX epoch
        System.TimeSpan timeSinceUnixEpoch = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1));
        Random.InitState(timeSinceUnixEpoch.Seconds);
    }

    private void Update()
    {
        // Car is "dead", cannot do any updates for this frame
        if (!m_IsAlive)
        {
            return;
        }

        // Current node of the waypoint system
        Transform thisNode = m_WaypointContainer.GetChild(m_MainTrackWaypointIndex);

        // Used when a way point is a child of an existing way point (nested track)
        Transform nestedThisNode = null;

        // By default, use the current node as the target to steer towards
        Vector3 targetPosition = thisNode.position;

        if (thisNode.gameObject.CompareTag(m_TrackSplitTag))
        {
            // Randomly decide left of right
            if (m_SplitChoice == (int)TrackSplitOptions.Invalid)
                m_SplitChoice = Random.Range(0, 2);
            
            // m_SplitChoice == 0 --> left
            // m_SplitChoice == 1 --> right
            nestedThisNode = thisNode.GetChild(m_SplitChoice);

            // Use a new target (child of the child node)
            targetPosition = nestedThisNode.GetChild(m_NestedTrackWaypointIndex).position;

            // Move on to the next way point once the car gets close
            UpdateNestedTrackWayPoint(targetPosition, nestedThisNode);
        }
        else if (thisNode.gameObject.CompareTag(m_PitSplitTag))
        {
            // Enter the pit lane if the health value falls below the acceptable amount
            if (m_Health.GetCurrentHealthValue() < m_Health.GetMaximumHealthValue() * m_PitLaneHealthThreshold)
            {
                // First child node of the sub-track container node
                nestedThisNode = thisNode.GetChild(m_NestedTrackWaypointIndex);

                // Use a new target
                targetPosition = nestedThisNode.position;

                // Move on to the next way point once the car gets close
                UpdateNestedTrackWayPoint(targetPosition, thisNode);
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

    // Check whether the car passed a way point in the current sub-track and if so, move on to the next nested way point
    private void UpdateNestedTrackWayPoint(Vector3 target, Transform node)
    {
        // Continue on the sub track
        if (Vector3.Distance(transform.position, target) < m_NextWaypointSelectionDistance)
            ++m_NestedTrackWaypointIndex;

        // Reached the end of the sub track
        if (m_NestedTrackWaypointIndex > node.childCount - 1)
            ++m_MainTrackWaypointIndex;
    }

    private void FixedUpdate()
    {
        // Make the AI feel more like a human player by giving it a random error margin
        m_SteeringValue *= Random.Range(m_MinimumErrorMargin, m_MaximumErrorMargin);

        // The steering value is in the -1 to 1 range, this is not usable for the throttle Lerp below.
        // To determine whether the car steers a lot (close to 1), or not (close to 0), the absolute value is needed.
        // Throttle is determined by the steering value, if the car barely has to steer, we can assume it is on a fairly
        // straight stretch of the track...
        float throttle = Mathf.Lerp(m_FullThrottle, m_BrakeThrottle, Mathf.Abs(m_SteeringValue));

        // Make the AI feel more like a human player by giving it a random error margin
        throttle *= Random.Range(m_MinimumErrorMargin, m_MaximumErrorMargin);

        // Apply throttle
        m_CarSuspension.Drive(throttle);

        // Apply steering towards the next way point
        m_CarSuspension.Steer(m_SteeringValue);
    }
}
