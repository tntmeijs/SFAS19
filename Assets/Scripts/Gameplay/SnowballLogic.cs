using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballLogic : MonoBehaviour
{
    // The lifetime of the bullet
    [SerializeField]
    float m_BulletLifeTime = 2.0f;

    [SerializeField] private LayerMask m_GroundLayer = 0;

    // The speed of the bullet
    [SerializeField]
    protected float m_BulletSpeed = 15.0f;

    // The damage of the bullet
    [SerializeField]
    int m_Damage = 20;

    bool m_Active = true;

    // Use this for initialization
    void Start()
    {
        // Add velocity to the bullet
        GetComponent<Rigidbody>().velocity = transform.forward * m_BulletSpeed;
    }

    // Update is called once per frame
    void Update ()
    {
        m_BulletLifeTime -= Time.deltaTime;
        if(m_BulletLifeTime < 0.0f)
        {
            Impact();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // The layermask returns a value, but this does not give the index as seen in the editor. To convert from an
        // integer value to the bit offset, the following calculation can be used: Log(layerValue, 2).
        int groundLayerMaskValueConverted = (int)Mathf.Log(m_GroundLayer.value, 2.0f);

        // Do nothing if the snowball is no longer alive, or if it hits the ground, it should just roll on the ground
        if(!m_Active ||
            collision.gameObject.layer == groundLayerMaskValueConverted)
        {
            return;
        }

        Health health = collision.gameObject.GetComponent<Health>();
        if(health)
        {
            health.DoDamage(m_Damage);
        }

        Impact();
        m_Active = false;
    }

    void Impact()
    {
        Explode();
        Destroy(gameObject);
    }

    protected virtual void Explode()
    {
    }
}
