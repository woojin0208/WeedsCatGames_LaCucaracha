using System.Collections.Generic;
using UnityEngine;

// 게임에서 사용하는 무기 프리팹 목록과 획득 기록을 관리한다.
[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [SerializeField]
    private WeaponBase[] weaponsPrefabs;
    public List<string> ObtainedWeaponIds { get; private set; } = new List<string>();

    public bool TryGetWeaponPrefab(string weaponId, out WeaponBase prefab)
    {
        prefab = null;

        if (string.IsNullOrWhiteSpace(weaponId))
        {
            Debug.LogWarning("[WeaponData] weaponId 가 비어 있습니다.", this);
            return false;
        }

        if (weaponsPrefabs == null || weaponsPrefabs.Length == 0)
        {
            Debug.LogWarning("[WeaponData] weaponPrefabs가 비어 있습니다.", this);
            return false;
        }

        for (int i = 0; i < weaponsPrefabs.Length; i++)
        {
            WeaponBase weapon = weaponsPrefabs[i];
            if (weapon == null || weapon.WeaponDefinition == null) continue;

            if (weapon.WeaponDefinition.WeaponId == weaponId)
            {
                prefab = weapon;
                return true;
            }
        }

        Debug.LogWarning($"[WeaponData] weaponId에 맞는 무기 프리팹이 없습니다.", this);
        return false;
    }

    // 무기를 처음 획득했는지 확인하고 획득 목록에 등록한다.
    public bool TryRegisterWeapon(WeaponBase weapon)
    {
        if (weapon == null || weapon.WeaponDefinition == null) return false;

        string weaponId = weapon.WeaponDefinition.WeaponId;
        if (string.IsNullOrWhiteSpace(weaponId)) return false;

        if (ObtainedWeaponIds.Contains(weaponId)) return false;

        ObtainedWeaponIds.Add(weaponId);
        return true;
    }

    // 획득 무기 목록을 초기화한다.
    public void ResetObtainedWeapons() => ObtainedWeaponIds = new List<string>();
}