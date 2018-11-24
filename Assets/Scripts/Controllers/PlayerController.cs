using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("References")]
    [SerializeField] private InputManager m_InputManager = null;

    [Header("Configuration")]
    [SerializeField] private float m_CharacterWalkingSpeed = 2.5f;
    [SerializeField] private float m_GravityStrength = 60.0f;
    [SerializeField] private float m_MaxFallSpeedOfCharacter = 20.0f;
    [SerializeField] private float m_JumpHeight = 4.0f;
    [SerializeField] private float m_DashForceMultiplier = 12.5f;
    [SerializeField] private float m_DashCooldownTime = 2.0f;

    // --------------------------------------------------------------

    private CharacterController m_CharacterController;

    // Stores the vertical and horizontal input values
    private Vector3 m_MovementInputXZ = Vector3.zero;

    // Holds a reference to the position where the player was at the start of the match.
    // This will be used to re-spawn the player at that location after a death.
    // TODO: Create a proper re-spawning system so this stuff can be removed.
    private Vector3 m_PlayerStartingPosition = Vector3.zero;

    private Vector3 m_AimPoint = Vector3.zero;

    private float m_CurrentMovementSpeed = 0.0f;

    // The current vertical / falling speed
    private float m_VerticalSpeed = 0.0f;
    
    private bool m_IsAlive = true;
    private bool m_AllowDash = true;

    // The time it takes to re-spawn
    private const float MAX_RESPAWN_TIME = 1.0f;
    private float m_RespawnTime = MAX_RESPAWN_TIME;

    // The force added to the player (used for knock backs)
    private Vector3 m_Force = Vector3.zero;

    // --------------------------------------------------------------

    public void Die()
    {
        m_IsAlive = false;
        m_RespawnTime = MAX_RESPAWN_TIME;
    }

    public void AddForce(Vector3 force)
    {
        m_Force += force;
    }

    public void SetAimPoint(Vector3 newAimPoint)
    {
        m_AimPoint = newAimPoint;
    }

    // --------------------------------------------------------------

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();

        bool allReferencesSetCorrectly = false;
        CheckReferencesForNull(out allReferencesSetCorrectly);

        if (!allReferencesSetCorrectly)
        {
            Debug.LogError("FATAL ERROR: Not all references have been set correctly!");
        }
    }

    private void Start()
    {
        m_PlayerStartingPosition = transform.position;

        // Hook up movement to the input manager
        m_InputManager.OnPlayerForwardInput     += InputForward;
        m_InputManager.OnPlayerBackwardInput    += InputBackward;
        m_InputManager.OnPlayerLeftInput        += InputLeft;
        m_InputManager.OnPlayerRightInput       += InputRight;
        
        // Hook up actions to the input manager (except for throwing)
        m_InputManager.OnPlayerDashInput    += InputDash;
        m_InputManager.OnPlayerJumpInput    += InputJump;
    }

    private void InputForward()
    {
        m_MovementInputXZ.z += 1.0f;
    }

    private void InputBackward()
    {
        m_MovementInputXZ.z += -1.0f;
    }

    private void InputLeft()
    {
        m_MovementInputXZ.x += -1.0f;
    }

    private void InputRight()
    {
        m_MovementInputXZ.x += 1.0f;
    }

    private void InputDash()
    {
        if (m_AllowDash)
        {
            Dash();
        }
    }

    private void Dash()
    {
        m_Force = m_MovementInputXZ.normalized * m_DashForceMultiplier;

        m_AllowDash = false;

        // Dash cooldown timer
        StartCoroutine(ApplyDashCooldown());
    }

    private IEnumerator ApplyDashCooldown()
    {
        yield return new WaitForSeconds(m_DashCooldownTime);
        m_AllowDash = true;
    }

    private void InputJump()
    {
        // Character can only jump when standing on the ground
        if (m_CharacterController.isGrounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        m_VerticalSpeed = Mathf.Sqrt(m_JumpHeight * m_GravityStrength);
    }

    private void CheckReferencesForNull(out bool status)
    {
        status = true;

        if (!m_InputManager)
        {
            status = false;
        }
    }
 
    // Update is called once per frame
    private void Update()
    {
        // If the player is dead update the re-spawn timer and exit update loop
        if(!m_IsAlive)
        {
            UpdateRespawnTime();
            return;
        }

        // Update movement input
        GatherInput();

        // Apply gravity
        ApplyGravity();

        // Move the player relative to the world
        ApplyCharacterMotion();
        
        // Rotate the character towards the target
        RotateCharacterInTargetDirection();

        // At the end of each frame, the input should reset to prevent the
        // player from moving non-stop
        m_MovementInputXZ = Vector3.zero;
    }

    private void UpdateRespawnTime()
    {
        m_RespawnTime -= Time.deltaTime;
        if (m_RespawnTime < 0.0f)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        m_IsAlive = true;

        ResetPlayerTransform();
    }

    private void ResetPlayerTransform()
    {
        transform.position = m_PlayerStartingPosition;
        transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    }

    private void GatherInput()
    {
        // Movement speed
        SetMovementSpeed();
    }

    private void SetMovementSpeed()
    {
        m_CurrentMovementSpeed = m_CharacterWalkingSpeed;
    }

    private void ApplyGravity()
    {
        // Apply gravity
        m_VerticalSpeed -= m_GravityStrength * Time.deltaTime;

        // Make sure we don't fall any faster than m_MaxFallSpeed.
        m_VerticalSpeed = Mathf.Max(m_VerticalSpeed, -m_MaxFallSpeedOfCharacter);
        m_VerticalSpeed = Mathf.Min(m_VerticalSpeed, m_MaxFallSpeedOfCharacter);
    }

    private void ApplyCharacterMotion()
    {
        // Calculate actual motion
        Vector3 m_CurrentMovementOffset = (m_MovementInputXZ.normalized * m_CurrentMovementSpeed + m_Force + new Vector3(0, m_VerticalSpeed, 0)) * Time.deltaTime;

        m_Force *= 0.95f;

        // Move character
        m_CharacterController.Move(m_CurrentMovementOffset);
    }

    private void RotateCharacterInTargetDirection()
    {
        // The player should never aim upward or downwards, so the Y component has to be the same when calculating
        // the aim direction.
        Vector3 aimPointWithoutY = new Vector3(m_AimPoint.x, transform.position.y, m_AimPoint.z);

        transform.forward = aimPointWithoutY - transform.position;
    }
}
