using System;
using UnityEngine;

public enum KeyType
{
    Attack = 0,
    Jump = 1,
    Dash = 2,
    Up = 3,
    Down = 4,
    Left = 5,
    Right = 6,
    Throw = 7,
    Interaction = 8,
    KeyCount = 9
}

[Serializable]
public class GameplayKeyBinding
{
    [SerializeField] private KeyCode attack = KeyCode.Mouse0;
    [SerializeField] private KeyCode jump = KeyCode.Space;
    [SerializeField] private KeyCode dash = KeyCode.LeftShift;
    [SerializeField] private KeyCode up = KeyCode.W;
    [SerializeField] private KeyCode down = KeyCode.S;
    [SerializeField] private KeyCode left = KeyCode.A;
    [SerializeField] private KeyCode right = KeyCode.D;
    [SerializeField] private KeyCode throwKey = KeyCode.E;
    [SerializeField] private KeyCode interaction = KeyCode.Mouse1;

    public KeyCode GetKey(KeyType keyType)
    {
        switch (keyType)
        {
            case KeyType.Attack: return attack;
            case KeyType.Jump: return jump;
            case KeyType.Dash: return dash;
            case KeyType.Up: return up;
            case KeyType.Down: return down;
            case KeyType.Left: return left;
            case KeyType.Right: return right;
            case KeyType.Throw: return throwKey;
            case KeyType.Interaction: return interaction;
            default: return KeyCode.None;
        }
    }

    public void SetKey(KeyType keyType, KeyCode keyCode)
    {
        switch (keyType)
        {
            case KeyType.Attack: attack = keyCode; break;
            case KeyType.Jump: jump = keyCode; break;
            case KeyType.Dash: dash = keyCode; break;
            case KeyType.Up: up = keyCode; break;
            case KeyType.Down: down = keyCode; break;
            case KeyType.Left: left = keyCode; break;
            case KeyType.Right: right = keyCode; break;
            case KeyType.Throw: throwKey = keyCode; break;
            case KeyType.Interaction: interaction = keyCode; break;
            default:
                throw new ArgumentOutOfRangeException(nameof(keyType), keyType, "Invalid gameplay key type.");
        }
    }
}

[Serializable]
public class PauseKeyBinding
{
    [SerializeField] private KeyCode pause = KeyCode.Escape;

    public KeyCode Pause => pause;
}

[Serializable]
public class DialogueKeyBinding
{
    [SerializeField] private KeyCode next = KeyCode.Space;
    [SerializeField] private KeyCode submit = KeyCode.Return;
    [SerializeField] private KeyCode up = KeyCode.W;
    [SerializeField] private KeyCode down = KeyCode.S;

    public KeyCode Next => next;
    public KeyCode Submit => submit;
    public KeyCode Up => up;
    public KeyCode Down => down;
}

[Serializable]
public class CutsceneKeyBinding
{
    [SerializeField] private KeyCode skip = KeyCode.Space;

    public KeyCode Skip => skip;
}

[CreateAssetMenu(fileName = "KeyBindingData", menuName = "Game/KeyBindingData")]
public class KeyBindingData : ScriptableObject
{
    [SerializeField] private GameplayKeyBinding gameplay = new GameplayKeyBinding();
    [SerializeField] private PauseKeyBinding pause = new PauseKeyBinding();
    [SerializeField] private DialogueKeyBinding dialogue = new DialogueKeyBinding();
    [SerializeField] private CutsceneKeyBinding cutscene = new CutsceneKeyBinding();

    public KeyCode GetGameplayKey(KeyType keyType) => gameplay.GetKey(keyType);

    public void SetGameplayKey(KeyType keyType, KeyCode keyCode) => gameplay.SetKey(keyType, keyCode);

    public KeyCode GetPauseKey() => pause.Pause;

    public KeyCode GetDialogueNextKey() => dialogue.Next;
    public KeyCode GetDialogueSubmitKey() => dialogue.Submit;
    public KeyCode GetDialogueUpKey() => dialogue.Up;
    public KeyCode GetDialogueDownKey() => dialogue.Down;

    public KeyCode GetCutsceneSkipKey() => cutscene.Skip;
}
