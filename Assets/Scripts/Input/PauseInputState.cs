using UnityEngine;

public class PauseInputState : IInputState
{
    private readonly KeyBindingData keyBindingData;

    public InputStateType StateType => InputStateType.Pause;

    public PauseInputState(KeyBindingData keyBindingData)
    {
        this.keyBindingData = keyBindingData;
    }

    public bool ResumePressed => Input.GetKeyDown(keyBindingData.GetPauseKey());
}
