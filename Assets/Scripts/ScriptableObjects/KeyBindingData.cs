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
public class KeyBinding
{
    [SerializeField] private KeyCode primary = KeyCode.None;
    [SerializeField] private KeyCode secondary = KeyCode.None;

    public KeyCode Primary => primary;
    public KeyCode Secondary => secondary;

    public bool GetKey()
    {
        return IsKey(primary) || IsKey(secondary);
    }

    public bool GetKeyDown()
    {
        return IsKeyDown(primary) || IsKeyDown(secondary);
    }

    public void SetPrimary(KeyCode keyCode)
    {
        primary = keyCode;
    }

    public void SetSecondary(KeyCode keyCode)
    {
        secondary = keyCode;
    }

    private bool IsKey(KeyCode keyCode)
    {
        return keyCode != KeyCode.None && Input.GetKey(keyCode);
    }

    private bool IsKeyDown(KeyCode keyCode)
    {
        return keyCode != KeyCode.None && Input.GetKeyDown(keyCode);
    }
}

[Serializable]
public class GameplayKeyBinding
{
    [SerializeField] private KeyBinding attack = new KeyBinding();
    [SerializeField] private KeyBinding jump = new KeyBinding();
    [SerializeField] private KeyBinding dash = new KeyBinding();
    [SerializeField] private KeyBinding up = new KeyBinding();
    [SerializeField] private KeyBinding down = new KeyBinding();
    [SerializeField] private KeyBinding left = new KeyBinding();
    [SerializeField] private KeyBinding right = new KeyBinding();
    [SerializeField] private KeyBinding throwKey = new KeyBinding();
    [SerializeField] private KeyBinding interaction = new KeyBinding();
    [SerializeField] private KeyBinding pause = new KeyBinding();

    public KeyBinding GetKey(KeyType keyType)
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
            default: throw new ArgumentOutOfRangeException(nameof(keyType), keyType, "Invalid gameplay key type.");
        }
    }

    public void SetKey(KeyType keyType, KeyCode keyCode)
    {
        switch (keyType)
        {
            case KeyType.Attack: attack.SetPrimary(keyCode); break;
            case KeyType.Jump: jump.SetPrimary(keyCode); break;
            case KeyType.Dash: dash.SetPrimary(keyCode); break;
            case KeyType.Up: up.SetPrimary(keyCode); break;
            case KeyType.Down: down.SetPrimary(keyCode); break;
            case KeyType.Left: left.SetPrimary(keyCode); break;
            case KeyType.Right: right.SetPrimary(keyCode); break;
            case KeyType.Throw: throwKey.SetPrimary(keyCode); break;
            case KeyType.Interaction: interaction.SetPrimary(keyCode); break;
            default:
                throw new ArgumentOutOfRangeException(nameof(keyType), keyType, "Invalid gameplay key type.");
        }
    }
}

[Serializable]
public class PauseKeyBinding
{
    [SerializeField] private KeyBinding pause = new KeyBinding();
    [SerializeField] private KeyBinding up = new KeyBinding();
    [SerializeField] private KeyBinding down = new KeyBinding();
    [SerializeField] private KeyBinding submit = new KeyBinding();
    public KeyBinding Pause => pause;
    public KeyBinding Up => up;
    public KeyBinding Down => down;
    public KeyBinding Submit => submit;

}

[Serializable]
public class DialogueKeyBinding
{
    [SerializeField] private KeyBinding next = new KeyBinding();
    [SerializeField] private KeyBinding submit = new KeyBinding();
    [SerializeField] private KeyBinding up = new KeyBinding();
    [SerializeField] private KeyBinding down = new KeyBinding();

    public KeyBinding Next => next;
    public KeyBinding Submit => submit;
    public KeyBinding Up => up;
    public KeyBinding Down => down;
}

[Serializable]
public class CutsceneKeyBinding
{
    [SerializeField] private KeyBinding skip = new KeyBinding();

    public KeyBinding Skip => skip;
}

[CreateAssetMenu(fileName = "KeyBindingData", menuName = "Game/KeyBindingData")]
public class KeyBindingData : ScriptableObject
{
    [SerializeField] private GameplayKeyBinding gameplay = new GameplayKeyBinding();
    [SerializeField] private PauseKeyBinding pause = new PauseKeyBinding();
    [SerializeField] private DialogueKeyBinding dialogue = new DialogueKeyBinding();
    [SerializeField] private CutsceneKeyBinding cutscene = new CutsceneKeyBinding();

    public KeyBinding GetGameplayBinding(KeyType keyType) => gameplay.GetKey(keyType);

    public void SetGameplayKey(KeyType keyType, KeyCode keyCode) => gameplay.SetKey(keyType, keyCode);

    public KeyBinding GetPauseBinding() => pause.Pause;
    public KeyBinding GetPauseUpBinding() => pause.Up;
    public KeyBinding GetPauseDownBinding() => pause.Down;
    public KeyBinding GetPauseSubmitBinding() => pause.Submit;

    public KeyBinding GetDialogueNextBinding() => dialogue.Next;
    public KeyBinding GetDialogueSubmitBinding() => dialogue.Submit;
    public KeyBinding GetDialogueUpBinding() => dialogue.Up;
    public KeyBinding GetDialogueDownBinding() => dialogue.Down;

    public KeyBinding GetCutsceneSkipBinding() => cutscene.Skip;
}
