using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SkateboardController : MonoBehaviour
{
    // Editor-exposed private member variables
    [Header("=== PHYSICS CONFIGURATION ===")]
    [SerializeField] private float m_HoverTargetHeight = 0.5f;
    [SerializeField] private float m_ForwardPushStrength = 1000.0f;
    [SerializeField] private float m_SteeringStrength = 1000.0f;
    [SerializeField] private float m_UpwardsForceWhenColiding = 1000.0f;
    [SerializeField] private float m_GravityForce = 1000.0f;
    [SerializeField] private float m_DragWhileGrounded = 3.25f;
    [SerializeField] private float m_DragWhileInAir = 0.125f;
    [SerializeField] private float m_AngularDrag = 4.0f;
    [SerializeField] private float m_Mass = 75.0f;
    [SerializeField] private LayerMask m_SkateboardLayerMask = 0;

    [Header("=== OBJECT REFERENCES ===")]
    [SerializeField] private List<GameObject> m_Wheels = new List<GameObject>();

    // Private member variables
    private Rigidbody m_SkateboardBoardRigidbody = null;
    private float m_SteeringValue = 0.0f;
    private float m_ForwardPushValue = 0.0f;

    private void Awake()
    {
        m_SkateboardBoardRigidbody = GetComponent<Rigidbody>();
        ConfigureRigidbody();

        // Ignore itself
        m_SkateboardLayerMask = ~m_SkateboardLayerMask;

        CheckAndLogErrors();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        // These gizmos help visualize the rays that are used to make the wheels hover above the ground
        foreach (GameObject go in m_Wheels)
        {
            Transform t = go.transform;
            Gizmos.DrawLine(t.position, t.position + -t.transform.up * m_HoverTargetHeight);
        }
    }

    private void FixedUpdate()
    {
        foreach (GameObject wheel in m_Wheels)
        {
            bool boardGrounded = false;

            RaycastHit hitInfo;
            Ray ray = new Ray(wheel.transform.position, -wheel.transform.up);

            if (Physics.Raycast(ray, out hitInfo, m_HoverTargetHeight, m_SkateboardLayerMask))
            {
                // The way the board floats is by using the ray as a spring system.
                // If the ray hits a collider, the board is getting too close to the ground.
                // The force that pushes the wheel back up should be relative to the "compression" amount of the spring.
                // If a ray is cast and the distance to the ground is small, a relatively big force should be applied in
                // the normal direction of the surface. It works exactly the other way around for rays that hit an object
                // from far away, the force that is applied to the wheel should be relatively small in this case.
                float springCompression = 1.0f - (hitInfo.distance / m_HoverTargetHeight);
                float springForce = springCompression * m_UpwardsForceWhenColiding * Time.fixedDeltaTime;

                boardGrounded = true;

                m_SkateboardBoardRigidbody.AddForceAtPosition(hitInfo.normal * springForce, wheel.transform.position, ForceMode.Force);
            }
            else
            {
                // Apply gravity
                m_SkateboardBoardRigidbody.AddForceAtPosition(Vector3.down * m_GravityForce * Time.fixedDeltaTime, wheel.transform.position, ForceMode.Force);
            }

            // The drag value should only slow down the board while it is on the ground.
            // This prevents the board from vibrating non-stop. Using such a high drag value while the board is
            // airborne, will make the gravity look off. Therefore, the drag has to change based on the current
            // state of the board.
            if (boardGrounded)
            {
                m_SkateboardBoardRigidbody.drag = m_DragWhileGrounded;
            }
            else
            {
                m_SkateboardBoardRigidbody.drag = m_DragWhileInAir;
            }

            // Apply forward / backward movement
            if (m_ForwardPushValue != 0.0f)
            {
                m_SkateboardBoardRigidbody.AddRelativeForce(wheel.transform.forward * m_ForwardPushValue * Time.fixedDeltaTime);
            }

            // Apply steering to the board
            if (m_SteeringValue != 0.0f)
            {
                m_SkateboardBoardRigidbody.AddRelativeTorque(wheel.transform.up * m_SteeringValue * Time.fixedDeltaTime);
            }
        }
    }

    private void CheckAndLogErrors()
    {
        if (m_Wheels.Count == 0)
        {
            Debug.LogError("FATAL ERROR: No wheel assigned to the skateboard!");
        }
    }

    private void ConfigureRigidbody()
    {
        // This function makes sure that the engine editor has no power over the rigidbody configuration.
        // Because the systems should all be balanced delicately, the decision has been made to let this script
        // be the only thing that can affect the state of the rigidbody component.
        m_SkateboardBoardRigidbody.mass = m_Mass;
        m_SkateboardBoardRigidbody.drag = m_DragWhileInAir;
        m_SkateboardBoardRigidbody.angularDrag = m_AngularDrag;
        m_SkateboardBoardRigidbody.useGravity = false;
        m_SkateboardBoardRigidbody.isKinematic = false;
        m_SkateboardBoardRigidbody.constraints = RigidbodyConstraints.None;
        m_SkateboardBoardRigidbody.interpolation = RigidbodyInterpolation.None;
        m_SkateboardBoardRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }
    
    public void SteerLeft(float normalizedStrength)
    {
        m_SteeringValue = m_SteeringStrength * normalizedStrength;
    }

    public void SteerRight(float normalizedStrength)
    {
        m_SteeringValue = m_SteeringStrength * normalizedStrength;
    }

    public void PushForward(float normalizedStrength)
    {
        m_ForwardPushValue = m_ForwardPushStrength * normalizedStrength;
    }
}
