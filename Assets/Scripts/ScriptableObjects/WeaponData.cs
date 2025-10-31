using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/WeaponData")]
public class WeaponData : ScriptableObject
{
    [SerializeField]
    private WeaponBase[] weaponsPrefabs;
    public List<string> ObtainedWeapons { get; private set; } = new List<string>(); // 이미 획득한 무기

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

    /// <summary>
    /// 무기가 첫 획득인지 체크하는 함수.
    /// </summary>
    /// <param name="weapon"></param>
    /// <returns></returns>
    public bool TryRegisterWeapon(WeaponBase weapon)
    {
        string weaponName = weapon.WeaponDefinition.WeaponName;
        if (ObtainedWeapons.Contains(weaponName)) return false;
        
        ObtainedWeapons.Add(weaponName);
        return true;
    }

    /// <summary>
    /// ObtainsWeapons 초기화.
    /// 추후 Save 추가 시, 변경
    /// </summary>
    public void ResetOptainWeapons() => ObtainedWeapons = new List<string>();
}
