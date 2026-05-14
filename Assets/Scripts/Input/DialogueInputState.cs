using UnityEngine;

public class DialogueInputState : IInputState
{
    private readonly KeyBindingData keyBindingData;

    public InputStateType StateType => InputStateType.Dialogue;

    public DialogueInputState(KeyBindingData keyBindingData)
    {
        this.keyBindingData = keyBindingData;
    }

    public bool NextPressed => keyBindingData.GetDialogueNextBinding().GetKeyDown();
    public bool SubmitPressed => keyBindingData.GetDialogueSubmitBinding().GetKeyDown();
    public bool UpPressed => keyBindingData.GetDialogueUpBinding().GetKeyDown();
    public bool DownPressed => keyBindingData.GetDialogueDownBinding().GetKeyDown();
}
