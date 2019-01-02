using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSuspension : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Wheels and suspension")]
    // Transforms from which the suspension rays will originate
    [SerializeField]
    private Transform[] m_Suspension;

    // Wheel objects, these will be placed on the floor once the rays hit. This gives the appearance of wheel suspension
    [SerializeField]
    private Transform[] m_Wheels;

    // Diameter of the wheels, used to calculate the proper offset from the ground
    [SerializeField]
    private float m_WheelDiameter = 0.25f;

    [Header("Suspension physics")]
    // Offset that will be applied to the center of mass
    [SerializeField]
    private Vector3 m_CenterOfMassOffset;

    // Length of the suspension ray
    [SerializeField]
    private float m_SuspensionRayLength = 0.5f;

    // Preferred height at which the vehicle should try to stay
    [SerializeField]
    private float m_TargetSuspensionHeight = 0.25f;

    // Suspension stiffness
    [SerializeField]
    private float m_SpringForce = 400.0f;

    // Force applied in the forward direction to simulate driving
    [SerializeField]
    private float m_DriveForce = 100.0f;

    // Torque is used to steer the vehicle
    [SerializeField]
    private float m_SteerForce = 100.0f;

    [Header("Miscellaneous physics")]
    // Drag applied to the rigidbody when the vehicle is not on the ground
    [SerializeField]
    private float m_DragInAir = 1.0f;

    // Angular drag applied to the rigidbody when the vehicle is not on the ground
    [SerializeField]
    private float m_AngularDragInAir = 1.0f;

    // Drag applied to the rigidbody when the vehicle is on the ground
    [SerializeField]
    private float m_DragWhileGrounded = 5.0f;

    // Angular drag applied to the rigidbody when the vehicle is on the ground
    [SerializeField]
    private float m_AngularDragWhileGrounded = 5.0f;

    // --------------------------------------------------------------

    private float m_DriveInput;
    private float m_SteerInput;

    private int m_GroundedWheelCounter;

    private Rigidbody m_Rigidbody;

#if UNITY_EDITOR
    private Vector3 m_OriginalCOM;
#endif

    // --------------------------------------------------------------

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        // Only used when using Gizmos to visualize the center of mass
#if UNITY_EDITOR
        m_OriginalCOM = m_Rigidbody.centerOfMass;
#endif

        m_Rigidbody.centerOfMass += m_CenterOfMassOffset;
    }

    private void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        // Reset the input from the previous frame
        ResetInput();

        // Retrieve the player input data from the input manager
        Global.PlayerInputData inputData = InputManager.instance.GetInputDataForPlayer(Global.Player.PlayerOne);

        // No input allowed for this frame
        if (!IsInputAllowed(inputData))
            return;

        if (inputData.buttonA)
        {
            // Drive
            m_DriveInput = 1.0f;
        }
        else if (inputData.buttonB)
        {
            // Reverse
            m_DriveInput = -1.0f;
        }

        if (m_DriveInput > 0)
        {
            // Driving forwards, use regular steering behavior
            m_SteerInput = inputData.axisLeftStickHorizontal;
        }
        else if (m_DriveInput < 0)
        {
            // Driving backwards, use inverted steering behavior
            m_SteerInput = -inputData.axisLeftStickHorizontal;
        }
    }

    private void ResetInput()
    {
        m_DriveInput = 0.0f;
        m_SteerInput = 0.0f;
    }

    private bool IsInputAllowed(Global.PlayerInputData inputData)
    {
        // No need to continue if there is not controller
        if (inputData.controller == Global.Controllers.None)
            return false;

        // Cannot control the car while it is airborne
        if (m_GroundedWheelCounter != m_Suspension.Length)
            return false;

        // All good to go
        return true;
    }

    private void FixedUpdate()
    {
        m_GroundedWheelCounter = 0;

        bool[] isGrounded = { false, false, false, false };

        // Apply steering
        m_Rigidbody.AddTorque(m_SteerInput * transform.up * m_SteerForce, ForceMode.Force);

        // Apply movement
        m_Rigidbody.AddForce(m_DriveInput * transform.forward * m_DriveForce, ForceMode.Force);

        // Apply hovering
        for (int i = 0; i < m_Suspension.Length; ++i)
        {
            RaycastHit hitInfo;

            if (Physics.Raycast(m_Suspension[i].position, -m_Suspension[i].up, out hitInfo, m_SuspensionRayLength))
            {
                float compressionRatio = m_TargetSuspensionHeight - (Vector3.Distance(hitInfo.point, m_Suspension[i].position) / m_TargetSuspensionHeight);

                m_Rigidbody.AddForceAtPosition(Vector3.up * m_SpringForce * compressionRatio, m_Suspension[i].position, ForceMode.Force);

                m_Wheels[i].position = hitInfo.point + new Vector3(0.0f, m_WheelDiameter, 0.0f);

                isGrounded[i] = true;
            }
            else
            {
                isGrounded[i] = false;
            }
        }

        foreach (bool grounded in isGrounded)
        {
            if (!grounded)
                break;

            ++m_GroundedWheelCounter;
        }

        if (m_GroundedWheelCounter == isGrounded.Length)
        {
            // All wheels grounded
            m_Rigidbody.drag = m_DragWhileGrounded;
            m_Rigidbody.angularDrag = m_AngularDragWhileGrounded;
        }
        else
        {
            // All wheels airborne
            m_Rigidbody.drag = m_DragInAir;
            m_Rigidbody.angularDrag = m_AngularDragInAir;
        }
    }

    // Only include the Gizmo rendering code in the Unity editor build
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_Rigidbody)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(transform.position + m_OriginalCOM + m_CenterOfMassOffset, new Vector3(0.05f, 0.05f, 0.05f));
        }

        foreach (var hoverpoint in m_Suspension)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hoverpoint.position, 0.05f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(hoverpoint.position, hoverpoint.position + -hoverpoint.up * m_SuspensionRayLength);
        }
    }
#endif
}
