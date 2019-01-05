using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // --------------------------------------------------------------

    // The Z Distance from the Camera Target
    [SerializeField]
    private float m_CameraDistanceZ = 15.0f;

    // --------------------------------------------------------------

    // The Camera Target
    private Transform m_PlayerTransform;

    // --------------------------------------------------------------

    // Directly set the camera follow target
    public void SetTargetTransform(Transform target)
    {
        m_PlayerTransform = target;
    }

    // --------------------------------------------------------------

    private void Start ()
    {
	}
	
	private void Update ()
    {
        transform.position = new Vector3(m_PlayerTransform.position.x, transform.position.y, m_PlayerTransform.position.z - m_CameraDistanceZ);
	}
}
