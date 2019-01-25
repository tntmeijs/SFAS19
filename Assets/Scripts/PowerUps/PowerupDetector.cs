using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PowerupDetector : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Name of the tag associated with the racing cars
    [SerializeField]
    private string m_CarTag = "Car";

    // What kind of power-up is this?
    [SerializeField]
    private PowerupManager.PowerupType m_PowerUpType = PowerupManager.PowerupType.None;

    // --------------------------------------------------------------

    // --------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        // The car has a collider attached to the body, which lives in a container, which is a child of the root
        // So like this: collider >>> container >>> root
        var carRoot = other.transform.parent.parent.gameObject;

        if (carRoot.CompareTag(m_CarTag))
        {
            // Let the player know which type of power-up they collected
            carRoot.gameObject.GetComponent<PowerupManager>().SetPowerUp(m_PowerUpType);

            // "Consume" the power-up
            Destroy(gameObject);
        }
    }
}
