using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 인벤토리에 저장되는 무기 인스턴스 정보를 보관한다.
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