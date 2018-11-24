using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowPileLogic : MonoBehaviour
{
    [SerializeField]
    int m_SnowQuantity = 50;

    void OnTriggerEnter(Collider other)
    {
        ThrowLogic throwLogic = other.GetComponentInChildren<ThrowLogic>();

        if(throwLogic)
        {
            throwLogic.AddSnow(m_SnowQuantity);
            Destroy(gameObject);
        }
    }
}
