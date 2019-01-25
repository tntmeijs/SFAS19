using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PowerupBase : MonoBehaviour
{
    // --------------------------------------------------------------

    [SerializeField]
    private string m_CarTag = "Car";

    // --------------------------------------------------------------

    // --------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(m_CarTag))
            Destroy(gameObject);
    }
}
