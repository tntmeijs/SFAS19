using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ResetDetector : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Time between checks in seconds whether the car is no longer moving correctly
    [SerializeField]
    private float m_CheckDelay = 5.0f;

    // --------------------------------------------------------------

    // Object that contains the ideal racing line way points
    private Transform m_WaypointContainer = null;

    // --------------------------------------------------------------

    public void SetWaypointContainer(Transform container)
    {
        m_WaypointContainer = container;
    }

    private void Start()
    {
        StartCoroutine(ResetCarLoop());
    }

    private IEnumerator ResetCarLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_CheckDelay);

            // If the car is in some kind of trouble, reset its position and orientation
            if (CarShouldBeReset())
                ResetVehicle();
        }
    }

    private bool CarShouldBeReset()
    {
        return false;
    }

    private void ResetVehicle()
    {

    }
}
