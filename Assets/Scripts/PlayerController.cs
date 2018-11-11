using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // --------------------------------------------------------------

    // The character's running speed
    [SerializeField]
    private float m_RunSpeed = 5.0f;

    // The gravity strength
    [SerializeField]
    private float m_Gravity = 60.0f;

    // The maximum speed the character can fall
    [SerializeField]
    private float m_MaxFallSpeed = 20.0f;

    // The character's jump height
    [SerializeField]
    private float m_JumpHeight = 4.0f;

    // --------------------------------------------------------------

    // The charactercontroller of the player
    CharacterController m_CharacterController;

    // The current movement direction in x & z.
    private Vector3 m_MovementDirection = Vector3.zero;

    // The current movement speed
    private float m_MovementSpeed = 0.0f;

    // The current vertical / falling speed
    private float m_VerticalSpeed = 0.0f;

    // The current movement offset
    private Vector3 m_CurrentMovementOffset = Vector3.zero;

    // The starting position of the player
    private Vector3 m_SpawningPosition = Vector3.zero;

    // Whether the player is alive or not
    private bool m_IsAlive = true;

    // The time it takes to respawn
    private const float MAX_RESPAWN_TIME = 1.0f;
    private float m_RespawnTime = MAX_RESPAWN_TIME;

    // The force added to the player (used for knockbacks)
    private Vector3 m_Force = Vector3.zero;

    // --------------------------------------------------------------

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    // Use this for initialization
    private void Start()
    {
        m_SpawningPosition = transform.position;
    }

    private void Jump()
    {
        m_VerticalSpeed = Mathf.Sqrt(m_JumpHeight * m_Gravity);
    }

    private void ApplyGravity()
    {
        // Apply gravity
        m_VerticalSpeed -= m_Gravity * Time.deltaTime;

        // Make sure we don't fall any faster than m_MaxFallSpeed.
        m_VerticalSpeed = Mathf.Max(m_VerticalSpeed, -m_MaxFallSpeed);
        m_VerticalSpeed = Mathf.Min(m_VerticalSpeed, m_MaxFallSpeed);
    }

    private void UpdateMovementState()
    {
        // Get Player's movement input and determine direction and set run speed
        float horizontalInput = Input.GetAxisRaw("Horizontal_P1");
        float verticalInput = Input.GetAxisRaw("Vertical_P1");

        m_MovementDirection = new Vector3(horizontalInput, 0, verticalInput);
        m_MovementSpeed = m_RunSpeed;
    }

    private void UpdateJumpState()
    {
        // Character can jump when standing on the ground
        if (Input.GetButtonDown("Jump_P1") && m_CharacterController.isGrounded)
        {
            Jump();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // If the player is dead update the respawn timer and exit update loop
        if(!m_IsAlive)
        {
            UpdateRespawnTime();
            return;
        }

        // Update movement input
        UpdateMovementState();

        // Update jumping input and apply gravity
        UpdateJumpState();
        ApplyGravity();

        // Calculate actual motion
        m_CurrentMovementOffset = (m_MovementDirection * m_MovementSpeed + m_Force  + new Vector3(0, m_VerticalSpeed, 0)) * Time.deltaTime;

        m_Force *= 0.95f;

        // Move character
        m_CharacterController.Move(m_CurrentMovementOffset);

        // Rotate the character towards the mouse cursor
        RotateCharacterTowardsMouseCursor();
    }

    private float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    private void RotateCharacter(Vector3 movementDirection)
    {
        Quaternion lookRotation = Quaternion.LookRotation(movementDirection);
        if (transform.rotation != lookRotation)
        {
            transform.rotation = lookRotation;
        }
    }

    private void RotateCharacterTowardsMouseCursor()
    {
        Vector3 mousePosInScreenSpace = Input.mousePosition;
        Vector3 playerPosInScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 directionInScreenSpace = mousePosInScreenSpace - playerPosInScreenSpace;

        float angle = Mathf.Atan2(directionInScreenSpace.y, directionInScreenSpace.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle + 90.0f, Vector3.up);
    }

    public void Die()
    {
        m_IsAlive = false;
        m_RespawnTime = MAX_RESPAWN_TIME;
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
        transform.position = m_SpawningPosition;
        transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    }

    public void AddForce(Vector3 force)
    {
        m_Force += force;
    }
}
