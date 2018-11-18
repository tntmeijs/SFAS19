using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    // --------------------------------------------------------------

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
    }

    private void Start()
    {
        m_PlayerStartingPosition = transform.position;
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

        // Update jumping input and apply gravity
        UpdateJumpState();
        ApplyGravity();

        // Update dashing input
        UpdateDashState();

        // Move the player relative to the world
        ApplyCharacterMotion();
        
        // Rotate the character towards the target
        RotateCharacterInTargetDirection();
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
        // Movement directions
        float horizontalInput = Input.GetAxisRaw("Horizontal_P1");
        float verticalInput = Input.GetAxisRaw("Vertical_P1");
        m_MovementInputXZ = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // Movement speed
        SetMovementSpeed();
    }

    private void SetMovementSpeed()
    {
        m_CurrentMovementSpeed = m_CharacterWalkingSpeed;
    }

    private void UpdateJumpState()
    {
        // Character can jump when standing on the ground
        if (Input.GetButtonDown("Jump_P1") && m_CharacterController.isGrounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        m_VerticalSpeed = Mathf.Sqrt(m_JumpHeight * m_GravityStrength);
    }

    private void ApplyGravity()
    {
        // Apply gravity
        m_VerticalSpeed -= m_GravityStrength * Time.deltaTime;

        // Make sure we don't fall any faster than m_MaxFallSpeed.
        m_VerticalSpeed = Mathf.Max(m_VerticalSpeed, -m_MaxFallSpeedOfCharacter);
        m_VerticalSpeed = Mathf.Min(m_VerticalSpeed, m_MaxFallSpeedOfCharacter);
    }

    private void UpdateDashState()
    {
        if (Input.GetButtonDown("Dash") && m_AllowDash)
        {
            m_Force = m_MovementInputXZ * m_DashForceMultiplier;

            m_AllowDash = false;

            StartCoroutine(ApplyDashCooldown());
        }
    }

    private IEnumerator ApplyDashCooldown()
    {
        yield return new WaitForSeconds(m_DashCooldownTime);
        m_AllowDash = true;
    }

    private void ApplyCharacterMotion()
    {
        // Calculate actual motion
        Vector3 m_CurrentMovementOffset = (m_MovementInputXZ * m_CurrentMovementSpeed + m_Force + new Vector3(0, m_VerticalSpeed, 0)) * Time.deltaTime;

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
