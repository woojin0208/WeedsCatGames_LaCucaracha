using UnityEngine;

// 무기 정적 정의 데이터를 보관한다.
// WeaponId 는 Save/Load와 런타임 조회에 사용하는 안정적인 ID이다.
// WeaponName은 UI에 표시하는 이름이다.
[CreateAssetMenu(fileName = "Definition_", menuName = "Game/Weapon/Weapon Definition")]
public class WeaponDefinition : ScriptableObject
{
    [field: SerializeField] public string WeaponId { get; private set; }
    [field: SerializeField] public Sprite WeaponIcon { get; private set; }
    [field: SerializeField, TextArea] public string WeaponName { get; private set; }
    [field: SerializeField, TextArea] public string WeaponDescript { get; private set; }
}