using UnityEngine;

public class GameplayInputState : IInputState
{
    private readonly KeyBindingData keyBindingData;
    private int blockedUntilFrame = -1;

    public InputStateType StateType => InputStateType.Gameplay;

    public GameplayInputState(KeyBindingData keyBindingData)
    {
        this.keyBindingData = keyBindingData;
    }

    public void BlockInputThisFrame()
    {
        blockedUntilFrame = Time.frameCount;
    }

    private bool IsBlockedThisFrame => Time.frameCount <= blockedUntilFrame;

    public float Horizontal =>
        IsBlockedThisFrame ? 0f :
        keyBindingData.GetGameplayBinding(KeyType.Left).GetKey() ? -1f :
        keyBindingData.GetGameplayBinding(KeyType.Right).GetKey() ? 1f : 0f;

    public bool JumpPressed => !IsBlockedThisFrame && keyBindingData.GetGameplayBinding(KeyType.Jump).GetKeyDown();
    public bool DashPressed => !IsBlockedThisFrame && keyBindingData.GetGameplayBinding(KeyType.Dash).GetKeyDown();
    public bool AttackPressed => !IsBlockedThisFrame && keyBindingData.GetGameplayBinding(KeyType.Attack).GetKeyDown();
    public bool InteractPressed => !IsBlockedThisFrame && keyBindingData.GetGameplayBinding(KeyType.Interaction).GetKeyDown();
    public bool ThrowPressed => !IsBlockedThisFrame && keyBindingData.GetGameplayBinding(KeyType.Throw).GetKeyDown();
    public bool PausePressed => !IsBlockedThisFrame && keyBindingData.GetPauseBinding().GetKeyDown();

    public int GetSelectWeaponNumber()
    {
        if (IsBlockedThisFrame) return -1;

        int maxCount = PlayerManager.Instance.MaxWeaponCount;
        for (int i = 0; i < maxCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) return i;
        }

        return -1;
    }

    public Vector2 MouseWorldPosition
    {
        get
        {
            Vector2 screenPosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0f;
            return worldPosition;
        }
    }
}
