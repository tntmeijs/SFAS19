using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // --------------------------------------------------------------

    [SerializeField] private KeyCode m_PlayerForwardKeyCode    = KeyCode.W;
    [SerializeField] private KeyCode m_PlayerBackwardKeyCode   = KeyCode.S;
    [SerializeField] private KeyCode m_PlayerLeftKeyCode       = KeyCode.A;
    [SerializeField] private KeyCode m_PlayerRightKeyCode      = KeyCode.D;
    [SerializeField] private KeyCode m_PlayerDashKeyCode       = KeyCode.LeftShift;
    [SerializeField] private KeyCode m_PlayerShootKeyCode      = KeyCode.Mouse0;

    // --------------------------------------------------------------

    public delegate void PlayerForwardInput();
    public delegate void PlayerBackwardInput();
    public delegate void PlayerLeftInput();
    public delegate void PlayerRightInput();
    public delegate void PlayerDashInput();
    public delegate void PlayerShootInput();

    public PlayerForwardInput   OnPlayerForwardInput;
    public PlayerBackwardInput  OnPlayerBackwardInput;
    public PlayerLeftInput      OnPlayerLeftInput;
    public PlayerRightInput     OnPlayerRightInput;
    public PlayerDashInput      OnPlayerDashInput;
    public PlayerShootInput     OnPlayerShootInput;

    // --------------------------------------------------------------

    public void SetPlayerForwardInputButton(KeyCode newKey)
    {
        m_PlayerForwardKeyCode = newKey;
    }

    public void SetPlayerBackwardInputButton(KeyCode newKey)
    {
        m_PlayerBackwardKeyCode = newKey;
    }

    public void SetPlayerLeftInputButton(KeyCode newKey)
    {
        m_PlayerLeftKeyCode = newKey;
    }

    public void SetPlayerRightInputButton(KeyCode newKey)
    {
        m_PlayerRightKeyCode= newKey;
    }

    public void SetPlayerDashInputButton(KeyCode newKey)
    {
        m_PlayerDashKeyCode = newKey;
    }

    public void SetPlayerShootInputButton(KeyCode newKey)
    {
        m_PlayerShootKeyCode = newKey;
    }

    // --------------------------------------------------------------

    private void Update()
    {
        if (Input.anyKey)
        {
            if (Input.GetKey(m_PlayerForwardKeyCode))
            {
                OnPlayerForwardInput();
            }

            if (Input.GetKey(m_PlayerBackwardKeyCode))
            {
                OnPlayerBackwardInput();
            }

            if (Input.GetKey(m_PlayerLeftKeyCode))
            {
                OnPlayerLeftInput();
            }

            if (Input.GetKey(m_PlayerRightKeyCode))
            {
                OnPlayerRightInput();
            }

            if (Input.GetKey(m_PlayerDashKeyCode))
            {
                OnPlayerDashInput();
            }

            if (Input.GetKey(m_PlayerShootKeyCode))
            {
                OnPlayerShootInput();
            }
        }
    }
}
