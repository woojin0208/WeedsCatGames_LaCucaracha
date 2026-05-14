using UnityEngine;

public class PauseInputState : IInputState
{
    private readonly KeyBindingData keyBindingData;

    public InputStateType StateType => InputStateType.Pause;

    public PauseInputState(KeyBindingData keyBindingData)
    {
        this.keyBindingData = keyBindingData;
    }

    public bool ResumePressed => keyBindingData.GetPauseBinding().GetKeyDown();
    public bool UpPressed => keyBindingData.GetPauseUpBinding().GetKeyDown();
    public bool DownPressed => keyBindingData.GetPauseDownBinding().GetKeyDown();
    public bool SubmitPressed => keyBindingData.GetPauseSubmitBinding().GetKeyDown();
}
