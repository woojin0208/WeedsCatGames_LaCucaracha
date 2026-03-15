using UnityEngine;

[System.Serializable]
public class GameplayKeyBinding
{
    [SerializeField] private KeyCode Attack = KeyCode.Mouse0;
    [SerializeField] private KeyCode Jump = KeyCode.Space;
    [SerializeField] private KeyCode Dash = KeyCode.LeftShift;
    [SerializeField] private KeyCode Up = KeyCode.W;
    [SerializeField] private KeyCode Down = KeyCode.S;
    [SerializeField] private KeyCode Left = KeyCode.A;
    [SerializeField] private KeyCode Right = KeyCode.D;
    [SerializeField] private KeyCode Throw = KeyCode.E;
    [SerializeField] private KeyCode Interaction = KeyCode.Mouse1;
    [SerializeField] private KeyCode Pause = KeyCode.Escape;
}

[System.Serializable]
public class DialogueKeyBinding
{
    [SerializeField] private KeyCode Next = KeyCode.Space;
    [SerializeField] private KeyCode Submit = KeyCode.Return;
    [SerializeField] private KeyCode Up = KeyCode.W;
    [SerializeField] private KeyCode Down = KeyCode.S;
}

[System.Serializable]
public class CutsceneKeyBinding
{
    [SerializeField] private KeyCode Skip = KeyCode.Space;
}
// 키 바인딩 기본값을 보관하고, SO로 런타임 Key
[CreateAssetMenu(fileName = "KeyBindingData", menuName = "Game/KeyBindingData")]
public class KeyBindingData : ScriptableObject
{
    public KeyCode[] keys = new KeyCode[(int)KeyType.KeyCount]
        {
            KeyCode.Mouse0, KeyCode.Space, KeyCode.LeftShift,
            KeyCode.W,      KeyCode.S,     KeyCode.A,
            KeyCode.D,      KeyCode.E,     KeyCode.Mouse1
        };

}