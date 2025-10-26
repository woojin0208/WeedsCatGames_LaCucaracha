using UnityEngine;

/// <summary>
/// Weapon Description UI¿ë WeaponDefinition
/// </summary>
[CreateAssetMenu(fileName = "WeaponDefinition", menuName = "Game/WeaponDefinition")]
public class WeaponDefinition : ScriptableObject
{
    [field: SerializeField] public Sprite WeaponIcon { get; private set; }
    [field: SerializeField, TextArea] public string WeaponName { get; private set; }
    [field: SerializeField, TextArea] public string WeaponDescript { get; private set; }
}
