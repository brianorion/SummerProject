using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Item
{
    public bool defenseWeapon;
    public float attackDamage;
    public float armor;
    public float magicResist;

    private void Start()
    {
        type = "Weapon";
    }
    public struct WeaponData
    {
        public bool defensiveType;
        public float attackDamage, armor, magicResist;

        public WeaponData(bool defensiveType, float attackDamage, float armor, float magicResist)
        {
            this.defensiveType = defensiveType;
            this.attackDamage = attackDamage;
            this.armor = armor;
            this.magicResist = magicResist;
        }

    }
}


