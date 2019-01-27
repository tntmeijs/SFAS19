using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapCounter : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    // Tag associated with the racing cars
    [SerializeField]
    private string m_CarTag = "Car";

    // --------------------------------------------------------------

    public delegate void CarPassesLapCounterVolume(Global.Player playerID, GameObject car);
    public CarPassesLapCounterVolume OnCarFinishesLap;

    // --------------------------------------------------------------
    
    private void OnTriggerEnter(Collider other)
    {
        // A car just passed the trigger volume
        if (other.gameObject.CompareTag(m_CarTag))
        {
            // Try to let any listeners know about the player that finished a lap
            try
            {
                // Retrieve the player ID (component is attached to the top-most object)
                OnCarFinishesLap(other.transform.root.GetComponent<PlayerID>().GetPlayerID(), other.transform.root.gameObject);
            }
            catch (System.NullReferenceException e)
            {
                // Ignore the exception, this only happens when there are no listeners attached to the delegate,
                // no big deal...
            }
        }
    }
}
