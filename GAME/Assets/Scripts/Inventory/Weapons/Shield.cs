using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Weapon
{
    private bool haveUpdatedShieldInformation = false;
    Dictionary<string, WeaponData> shieldData = new Dictionary<string, WeaponData>();

    private void Start()
    {
        haveUpdatedShieldInformation = true;
        WeaponData weaponData = new WeaponData(defenseWeapon, attackDamage, armor, magicResist);
        shieldData.Add(name, weaponData);
    }

    public Dictionary<string, WeaponData> GetShieldData()
    {
        return shieldData;
    }

}