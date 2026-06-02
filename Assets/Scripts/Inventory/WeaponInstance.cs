using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인벤토리에 저장되는 무기 인스턴스 정보다.
/// Id는 개별 무기 인스턴스 ID이고, WeaponId는 정적 무기 데이터 ID다.
/// </summary>
[Serializable]
public class WeaponInstance
{
    public string Id { get; private set; }
    public string WeaponId { get; private set; }
    public int Durability { get; set; }

    /// <summary>
    /// 새 무기를 처음 획득할 때 사용한다.
    /// 새로운 인스턴스 ID를 자동 생성한다.
    /// </summary>
    public WeaponInstance(string weaponId, int durability)
    {
        Id = Guid.NewGuid().ToString("N");
        WeaponId = weaponId;
        Durability = durability;
    }

    /// <summary>
    /// Save/Load로 저장된 무기 상태를 복원할 때 사용한다.
    /// 저장된 인스턴스 ID를 그대로 유지한다.
    /// </summary>
    public WeaponInstance(string id, string weaponId, int durability)
    {
        Id = id;
        WeaponId = weaponId;
        Durability = durability;
    }
}