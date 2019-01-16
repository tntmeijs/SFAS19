using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNearbyCarCount : StateMachineBehaviour
{
    // --------------------------------------------------------------

    [Header("State machine information")]
    // Name of the car count parameter
    [SerializeField]
    private string m_INT_NearbyCarCount = "INFO_NearbyCarCount";

    [Header("Tags")]
    // Name of the tag assigned to all cars
    [SerializeField]
    private string m_CarTag = "Car";

    // --------------------------------------------------------------

    // Detection trigger assigned to the car
    private SphereCollider m_NearbyDetector = null;

    // Number of cars near this car
    private int m_CarsNearby = 0;

    // --------------------------------------------------------------

    private void Awake()
    {
        //
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //
    }

    private void OnDisable()
    {
        //
    }
}
