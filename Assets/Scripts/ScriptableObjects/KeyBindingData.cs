using UnityEngine;

[CreateAssetMenu(fileName = "KeyBindingData", menuName = "Game/KeyBindingData")]
public class KeyBindingData : ScriptableObject // 현재 키 입력 값을 저장하는 스크립트.
{
    
    public KeyCode[] keys = new KeyCode[(int)KeyType.KeyCount]
        {
            KeyCode.Mouse0, KeyCode.Space, KeyCode.LeftShift,
            KeyCode.W,      KeyCode.S,     KeyCode.A,
            KeyCode.D,      KeyCode.E,     KeyCode.Mouse1
        };

}
