using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class AimInputHandler : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("References")]
    [SerializeField] private Camera m_MainCamera = null;

    // --------------------------------------------------------------

    private PlayerController m_PlayerController = null;

    // --------------------------------------------------------------

    private void Awake()
    {
        m_PlayerController = GetComponent<PlayerController>();

        bool referencesStatus = CheckForNullReferences();

        if (!referencesStatus)
        {
            Debug.LogError("One or more object references have not been set-up correctly!");
        }
    }

    private bool CheckForNullReferences()
    {
        if (!m_MainCamera)
        {
            return false;
        }

        return true;
    }

    private void Update()
    {
        if (Input.GetJoystickNames().Length == 0)
        {
            UpdateMouseInputForPlayer();
        }
        else
        {
            UpdateJoystickInputForPlayer();
        }
    }

    private void UpdateMouseInputForPlayer()
    {
        Ray ray = m_MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo = new RaycastHit();

        if (Physics.Raycast(ray, out hitInfo))
        {
            m_PlayerController.SetAimPoint(hitInfo.point);
        }
    }

    private void UpdateJoystickInputForPlayer()
    {
        Debug.LogWarning("Joystick input has not yet been implemented!");
    }
}
