using UnityEngine;

public class CutsceneInputState : IInputState
{
    private readonly KeyBindingData keyBindingData;

    public InputStateType StateType => InputStateType.CutScene;

    public CutsceneInputState(KeyBindingData keyBindingData)
    {
        this.keyBindingData = keyBindingData;
    }

    public bool SkipPressed => Input.GetKeyDown(keyBindingData.GetCutsceneSkipKey());
}
