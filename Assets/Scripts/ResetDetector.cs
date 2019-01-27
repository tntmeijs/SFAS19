using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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

    // If this is an AI controller car, set a minimum speed before it is assumed the AI is stuck
    [SerializeField]
    private float m_MinimumVelocity = 0.8f;

    // --------------------------------------------------------------

    // Object that contains the ideal racing line way points
    private Transform m_WaypointContainer = null;

    // Rigidbody attacked to this object
    private Rigidbody m_Rigidbody = null;

    // --------------------------------------------------------------

    public void SetWaypointContainer(Transform container)
    {
        m_WaypointContainer = container;
    }

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
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
        // The reason we do not cache this AIController reference is because when a player finishes, the player controller
        // component is switched by an ai controller component. When caching the AI controller component, you only update it
        // in Awake, which will result in a false-positive in that case.
        //
        // I could have created a getting / setter and do it like that, but I am short on time. When I have time, I will
        // implement that instead.
        // 
        // TODO: See comment above for optimization!
        // 
        // This is an AI controller, the AI is unable to wait on purpose, so assume it got stuck somewhere in the trees or whatever
        if (GetComponent<AIController>() && (m_Rigidbody.velocity.magnitude >= 0.0f && m_Rigidbody.velocity.magnitude <= m_MinimumVelocity))
            return true;

        // Car is slightly angled towards the ground, assume it is upside-down
        return (Vector3.Dot(transform.up, Vector3.down) > 0.0f);
    }

    private void ResetVehicle()
    {
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

            // Place the car back on track (slightly in the air to avoid clipping through the ground)
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
