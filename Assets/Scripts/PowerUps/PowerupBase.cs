using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PowerupBase : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Name of the tag associated with the racing cars
    [SerializeField]
    private string m_CarTag = "Car";

    // What kind of power-up is this?
    [SerializeField]
    private PowerupManager.PowerupType m_PowerUpType = PowerupManager.PowerupType.None;

    // Time until the power-up reappears again
    [SerializeField]
    private float m_PowerUpCooldown = 2.0f;

    // --------------------------------------------------------------

    private SphereCollider m_Trigger = null;
    private GameObject m_MeshContainer = null;

    // --------------------------------------------------------------

    private void Awake()
    {
        // No need to check if this is valid, it always succeeds because this script REQUIRES this component
        m_Trigger = GetComponent<SphereCollider>();

        // The mesh container object is the first child of the root (which holds this script)
        m_MeshContainer = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        // The car has a collider attached to the body, which lives in a container, which is a child of the root
        // So like this: collider >>> container >>> root
        var carRoot = other.transform.parent.parent.gameObject;

        if (carRoot.CompareTag(m_CarTag))
        {
            // Let the player know which type of power-up they collected
            carRoot.gameObject.GetComponent<PowerupManager>().SetPowerUp(m_PowerUpType);

            // Hide the power-up until the time-out finishes
            StartCoroutine(PowerUpTimeOut());
        }
    }

    private IEnumerator PowerUpTimeOut()
    {
        // Disable the mesh of the object and the collider on the root
        m_MeshContainer.SetActive(false);
        m_Trigger.enabled = false;

        yield return new WaitForSeconds(m_PowerUpCooldown);

        // Revert the object back to its default state
        m_MeshContainer.SetActive(true);
        m_Trigger.enabled = true;
    }
}
