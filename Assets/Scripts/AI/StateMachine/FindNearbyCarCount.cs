using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNearbyCarCount : StateMachineBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Detection radius around the car
    [SerializeField]
    private float m_SphereDetectorRadius = 10.0f;

    [Header("State machine information")]
    // Name of the car count parameter
    [SerializeField]
    private string m_INT_NearbyCarCount = "INFO_NearbyCarCount";

    // Name of the distance to the closest car parameter
    [SerializeField]
    private string m_FLOAT_ClosestDistanceToOtherCar = "INFO_ClosestDistanceToOtherCar";

    [Header("Tags")]
    // Name of the tag assigned to all cars
    [SerializeField]
    private string m_CarTag = "Car";

    // --------------------------------------------------------------

    // References have already been set
    private bool m_SetupCompleted = false;

    // --------------------------------------------------------------
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Only set references once
        if (!m_SetupCompleted)
            SetReferencesIfNotSet(animator.gameObject);
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Collider[] overlapping_objects = Physics.OverlapSphere(Vector3.zero, m_SphereDetectorRadius);

        int nearby_cars = 0;

        // Find the cars that are within range
        foreach (var collider in overlapping_objects)
        {
            if (collider.gameObject.CompareTag(m_CarTag))
                ++nearby_cars;
        }

        // Save the number of nearby cars in the state machine parameters
        animator.SetInteger(m_INT_NearbyCarCount, nearby_cars);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //
    }

    private void SetReferencesIfNotSet(GameObject gameObject)
    {
        // Never run the reference retrieval code again
        m_SetupCompleted = true;
    }
}
