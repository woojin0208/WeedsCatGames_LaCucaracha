using UnityEngine;

// 무기 정의 데이터를 보관한다.
public class WeaponDefinition : ScriptableObject
{
    [field: SerializeField] public Sprite WeaponIcon { get; private set; }
    [field: SerializeField, TextArea] public string WeaponName { get; private set; }
    [field: SerializeField, TextArea] public string WeaponDescript { get; private set; }
}