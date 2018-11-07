using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SkateboardController : MonoBehaviour
{
    // Editor-exposed private member variables
    [Header("=== PHYSICS CONFIGURATION ===")]
    [SerializeField] private float m_HoverTargetHeight = 0.5f;
    [SerializeField] private float m_WheelRayLengthInMeters = 0.1f;
    [SerializeField] private float m_UpwardsForceWhenColidingInNewton = 1000.0f;

    [Header("=== OBJECT REFERENCES ===")]
    [SerializeField] private List<GameObject> m_Wheels = new List<GameObject>();

    // Private member variables
    private Rigidbody m_SkateboardBoardRigidbody = null;

    private void Awake()
    {
        m_SkateboardBoardRigidbody = GetComponent<Rigidbody>();

        CheckAndLogErrors();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        // These gizmos help visualize the rays that are used to make the wheels hover above the ground
        foreach (GameObject go in m_Wheels)
        {
            Transform t = go.transform;
            Gizmos.DrawLine(t.position, t.position + -t.transform.up * m_WheelRayLengthInMeters);
        }
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        foreach (GameObject wheel in m_Wheels)
        {
            RaycastHit hitInfo;
            Ray ray = new Ray(wheel.transform.position, -wheel.transform.up);

            if (Physics.Raycast(ray, out hitInfo, m_WheelRayLengthInMeters))
            {
                // The way the board floats is by using the ray as a spring system.
                // If the ray hits a collider, the board is getting too close to the ground.
                // The force that pushes the wheel back up should be relative to the "compression" amount of the spring.
                // If a ray is cast and the distance to the ground is small, a relatively big force should be applied in
                // the normal direction of the surface. It works exactly the other way around for rays that hit an object
                // from far away, the force that is applied to the wheel should be relatively small in this case.
                float springCompression = (m_HoverTargetHeight - hitInfo.distance) / m_HoverTargetHeight;
                float springForce = springCompression * m_UpwardsForceWhenColidingInNewton * Time.fixedDeltaTime;

                m_SkateboardBoardRigidbody.AddForceAtPosition(hitInfo.normal * springForce, wheel.transform.position, ForceMode.Force);
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
}
