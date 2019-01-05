using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class HoleGravity : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Tag assigned to the ball
    [SerializeField]
    private string m_BallTag = "Ball";

    // Strength of the gravitational pull in Newton
    [SerializeField]
    private float m_GravityStrength = 10.0f;

    // --------------------------------------------------------------

    // Radius of the sphere collider attached to this game-object
    private float m_SphereColliderRadius = 0.0f;

    // --------------------------------------------------------------

    private void Awake()
    {
        // Retrieve the collider radius (always works because this script REQUIRES a sphere collider component)
        m_SphereColliderRadius = GetComponent<SphereCollider>().radius;
    }

    // According to the documentation, this function is called when the physics update, which means that it is safe to
    // do something with the physics engine within this function.
    // https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerStay.html
    private void OnTriggerStay(Collider other)
    {
        // The object in the volume is the ball
        if (other.gameObject.tag == m_BallTag)
        {
            // Distance from the center of the ball to the center of the "gravity volume"
            float distanceToBall = Vector3.Distance(other.transform.position, transform.position);

            // Direction to the center of the "gravity volume"
            Vector3 pullDirection = (transform.position - other.transform.position).normalized;

            // The strength of the pull force is based on the distance of the ball to the hole
            float finalPullStrength = m_GravityStrength * (distanceToBall / m_SphereColliderRadius);

            // Apply the force into the pull direction
            other.gameObject.GetComponent<Rigidbody>().AddForce(pullDirection * finalPullStrength);
        }
    }
}
