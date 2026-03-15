using UnityEngine;
using UnityEngine.InputSystem;

// 게임플레이 상태에서 사용하는 입력 조회를 담당한다.
public class GameplayInputState : IInputState
{
    private readonly KeyBindingData keyBindingData;

    public InputStateType StateType => InputStateType.Gameplay;

    public GameplayInputState(KeyBindingData keyBindingData)
    {
        this.keyBindingData = keyBindingData;
    }

    public float Horizontal =>
        Input.GetKey(keyBindingData.keys[(int)KeyType.Left]) ? -1f :
        Input.GetKey(keyBindingData.keys[(int)KeyType.Right]) ? 1f : 0f;

    public bool JumpPressed => Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Jump]);
    public bool DashPressed => Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Dash]);
    public bool AttackPressed => Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Attack]);
    public bool InteractPressed => Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Interaction]);
    public bool ThrowPressed => Input.GetKeyDown(keyBindingData.keys[(int)KeyType.Throw]);
    public bool PausePressed => Input.GetKeyDown(KeyCode.Escape);

    public int GetSelectWeaponNumber()
    {
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
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0f;
            return worldPosition;
        }
    }
}
