using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Input 상태의 종류를 정의한다.
public enum InputStateType
{
    Gameplay,
    Pause,
    Dialogue,
    CutScene
}

// 각각의 InputState가 제공해야 하는 읽기 전용 데이터를 정의한다.
public interface IInputState
{
    InputStateType StateType { get; }
}

// 입력 상태를 관리한다.
public class InputStateManager : MonoBehaviour
{
    [SerializeField] private KeyBindingData keyBindingData;

    public InputStateType CurrentStateType { get; private set; } = InputStateType.Gameplay;
    public IInputState CurrentState { get; private set; }
    private static InputStateManager instance;

    public static InputStateManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize(keyBindingData);
    }

    public void Initialize(KeyBindingData keyBindingData)
    {
        // 각 Input State 추가
    }
    public void ChangeState(InputStateType nextState) => CurrentStateType = nextState;

    public bool IsState(InputStateType state) => CurrentStateType == state;
}
