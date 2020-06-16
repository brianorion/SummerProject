using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grenade : Projectile
{
    public float explodeDelay = 3f;
    public float range = 5f;
    public float explosionForce = 10f;
    private bool haveUpdatedGrenadeInformation = false;
    Dictionary<string, WeaponData> grenadeData = new Dictionary<string, WeaponData>();
    private Rigidbody rb;


    private void Start()
    {
        haveUpdatedGrenadeInformation = true;
        WeaponData weaponData = new WeaponData(defenseWeapon, attackDamage, armor, magicResist);
        grenadeData.Add(name, weaponData);
        rb = GetComponent<Rigidbody>();
    }

    public Dictionary<string, WeaponData> GetGrenadeData()
    {
        return grenadeData;
    }

    override public void ThrowItem(Quaternion orientation)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.AddForce(orientation * Vector3.forward * launchForce);
        rb.AddTorque(Random.insideUnitSphere * maxTorque);
        Invoke("Explode", explodeDelay);
    }
    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range); //get all objects in radius of contact
        foreach (Collider hit in colliders)
        {
            Rigidbody rbHit = hit.GetComponent<Rigidbody>();

            if (rbHit != null)
                rbHit.AddExplosionForce(explosionForce, transform.position, range, 10.0F);
        }
        Destroy(gameObject);
    }
}