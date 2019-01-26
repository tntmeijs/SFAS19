using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class MissileBehavior : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Tag associated with missiles
    [SerializeField]
    private string m_MissileTag = "Missile";

    // Rotation in degrees per second towards the ground
    [SerializeField]
    [Range(5.0f, 90.0f)]
    private float m_DriveAngleIncrease = 50.0f;

    // Force added to the missile when it is flying through the air
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float m_ThrustForce = 50.0f;

    // --------------------------------------------------------------

    private Rigidbody m_Rigidbody = null;

    // --------------------------------------------------------------
    
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        m_Rigidbody.AddForce(transform.forward * m_ThrustForce, ForceMode.Force);

        // Make the rocket face downwards to make it fall towards the ground faster
        transform.Rotate(Vector3.right, m_DriveAngleIncrease * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore collisions with other missiles
        if (other.CompareTag(m_MissileTag))
            return;

        // TODO: Spawn explosion particle effect
        Destroy(gameObject);
    }
}
