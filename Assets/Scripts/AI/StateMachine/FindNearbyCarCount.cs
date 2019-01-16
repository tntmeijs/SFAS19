using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNearbyCarCount : StateMachineBehaviour
{
    // --------------------------------------------------------------

    [Header("State machine information")]
    // Name of the car count parameter
    [SerializeField]
    private const string m_INT_NearbyCarCount = "INFO_NearbyCarCount";

    // Name of the distance to the closest car parameter
    [SerializeField]
    private const string m_FLOAT_ClosestDistanceToOtherCar = "INFO_ClosestDistanceToOtherCar";

    [Header("Tags")]
    // Name of the tag assigned to all cars
    [SerializeField]
    private const string m_CarTag = "Car";

    // --------------------------------------------------------------

    // References have already been set
    private bool m_SetupCompleted = false;

    // Detection trigger assigned to the car
    private SphereCollider m_NearbyDetector = null;

    // --------------------------------------------------------------
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Only set references once
        if (!m_SetupCompleted)
            SetReferencesIfNotSet(animator.gameObject);

        Debug.Log("Enter");
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //
    }

    private void SetReferencesIfNotSet(GameObject gameObject)
    {
        // Never run the reference retrieval code again
        m_SetupCompleted = true;

        // Each car has a sphere trigger that can be used to detect the distance to the closest car
        m_NearbyDetector = gameObject.GetComponent<SphereCollider>();

        Debug.Log("References set");
    }
}
