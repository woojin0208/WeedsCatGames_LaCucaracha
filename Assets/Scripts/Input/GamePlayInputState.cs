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
        Input.GetKey(keyBindingData.GetGameplayKey(KeyType.Left)) ? -1f :
        Input.GetKey(keyBindingData.GetGameplayKey(KeyType.Right)) ? 1f : 0f;

    public bool JumpPressed => !IsBlockedThisFrame && Input.GetKeyDown(keyBindingData.GetGameplayKey(KeyType.Jump));
    public bool DashPressed => !IsBlockedThisFrame && Input.GetKeyDown(keyBindingData.GetGameplayKey(KeyType.Dash));
    public bool AttackPressed => !IsBlockedThisFrame && Input.GetKeyDown(keyBindingData.GetGameplayKey(KeyType.Attack));
    public bool InteractPressed => !IsBlockedThisFrame && Input.GetKeyDown(keyBindingData.GetGameplayKey(KeyType.Interaction));
    public bool ThrowPressed => !IsBlockedThisFrame && Input.GetKeyDown(keyBindingData.GetGameplayKey(KeyType.Throw));
    public bool PausePressed => !IsBlockedThisFrame && Input.GetKeyDown(keyBindingData.GetPauseKey());

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
