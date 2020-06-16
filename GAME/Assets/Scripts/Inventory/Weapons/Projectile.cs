using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Weapon
{
    public float maxTorque = 1000;
    public float launchForce = 700;
    private Rigidbody rb;

    void Start()
    {
        type = "Projectile";
        rb = GetComponent<Rigidbody>();
    }

    public virtual void ThrowItem(Quaternion orientation)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.AddForce(orientation * Vector3.forward * launchForce);
        rb.AddTorque(Random.insideUnitSphere * maxTorque);
    }
}