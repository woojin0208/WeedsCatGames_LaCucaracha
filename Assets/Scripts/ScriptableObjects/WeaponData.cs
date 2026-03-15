using System.Collections.Generic;
using UnityEngine;

// 게임에서 사용하는 무기 프리팹 목록과 획득 기록을 관리한다.
public class WeaponData : ScriptableObject
{
    [SerializeField]
    private WeaponBase[] weaponsPrefabs;
    public List<string> ObtainedWeapons { get; private set; } = new List<string>();

    public WeaponBase GetCurrentWeaponData(string weaponName)
    {
        foreach (var w in weaponsPrefabs)
        {
            if (w.name == weaponName)
            {
                return w;
            }
        }

        return null;
    }

    // 무기를 처음 획득했는지 확인하고 획득 목록에 등록한다.
    public bool TryRegisterWeapon(WeaponBase weapon)
    {
        string weaponName = weapon.WeaponDefinition.WeaponName;
        if (ObtainedWeapons.Contains(weaponName)) return false;
        
        ObtainedWeapons.Add(weaponName);
        return true;
    }

    // 획득 무기 목록을 초기화한다.
    public void ResetOptainWeapons() => ObtainedWeapons = new List<string>();
}