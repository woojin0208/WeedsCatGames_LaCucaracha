using UnityEngine;

public class DialogueInputState : IInputState
{
    private readonly KeyBindingData keyBindingData;

    public InputStateType StateType => InputStateType.Dialogue;

    public DialogueInputState(KeyBindingData keyBindingData)
    {
        this.keyBindingData = keyBindingData;
    }

    public bool NextPressed => Input.GetKeyDown(keyBindingData.GetDialogueNextKey());
    public bool SubmitPressed => Input.GetKeyDown(keyBindingData.GetDialogueSubmitKey());
    public bool UpPressed => Input.GetKeyDown(keyBindingData.GetDialogueUpKey()) || Input.GetKeyDown(KeyCode.UpArrow);
    public bool DownPressed => Input.GetKeyDown(keyBindingData.GetDialogueDownKey()) || Input.GetKeyDown(KeyCode.DownArrow);
}
