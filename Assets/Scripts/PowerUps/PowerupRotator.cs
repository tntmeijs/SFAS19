using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupRotator : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Rotations per second
    [SerializeField]
    private float m_RotationSpeed = 2.5f;

    // --------------------------------------------------------------

    protected void Update()
    {
        // It is a waste of resources to use an Animator for this, so this will be used to "animate" it instead
        RotateObject();
    }

    // Rotate the power-up along the world vertical axis
    private void RotateObject()
    {
        transform.Rotate(Vector3.up, 360.0f * m_RotationSpeed * Time.deltaTime, Space.World);
    }
}
