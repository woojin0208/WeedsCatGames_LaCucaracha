using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/WeaponData")]
public class WeaponData : ScriptableObject
{
    [SerializeField]
    private WeaponBase[] weaponsPrefabs;
    [SerializeField]
    private Sprite[] weaponIconSprites;
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
}
