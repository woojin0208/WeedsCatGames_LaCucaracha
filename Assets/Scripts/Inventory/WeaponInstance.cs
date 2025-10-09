using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInstance
{
    public readonly string Id;
    public string WeaponName { get; private set; }
    public int Durability { get; set; }

    public WeaponInstance(string weaponName, int durability)
    {
        Id = Guid.NewGuid().ToString("N");
        WeaponName = weaponName;
        Durability = durability;
    }
}