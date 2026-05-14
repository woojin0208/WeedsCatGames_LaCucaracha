using System;
using System.Collections.Generic;
using UnityEngine;

public enum InputStateType
{
    Gameplay,
    Pause,
    Dialogue,
    CutScene
}

public interface IInputState
{
    InputStateType StateType { get; }
}

public class InputStateManager : MonoBehaviour
{
    // 상태별 키 바인딩을 제공하는 데이터 에셋이다.
    [SerializeField] private KeyBindingData keyBindingData;

    // 상태 타입별 입력 상태 인스턴스를 보관한다.
    private readonly Dictionary<InputStateType, IInputState> states = new Dictionary<InputStateType, IInputState>();
    private static InputStateManager instance;

    // 입력 상태를 전역에서 접근하기 위한 싱글턴 인스턴스다.
    public static InputStateManager Instance
    {
        get { return instance; }
    }

    // 입력 설정 UI가 참조할 현재 바인딩 데이터다.
    public KeyBindingData BindingData => keyBindingData;
    // Gameplay 입력 상태 인스턴스다.
    public GameplayInputState GameplayState { get; private set; }
    // Pause 입력 상태 인스턴스다.
    public PauseInputState PauseState { get; private set; }
    // Dialogue 입력 상태 인스턴스다.
    public DialogueInputState DialogueState { get; private set; }
    // CutScene 입력 상태 인스턴스다.
    public CutsceneInputState CutsceneState { get; private set; }
    // 현재 활성화된 입력 상태 타입이다.
    public InputStateType CurrentStateType { get; private set; } = InputStateType.Gameplay;
    // 현재 활성화된 입력 상태 인스턴스다.
    public IInputState CurrentState { get; private set; }

    // Gameplay/Pause 상태에서 Pause 토글 요청을 발행한다.
    public event Action PauseToggleRequested;

    // Pause 상태에서 위 선택 이동 요청을 발행한다.
    public event Action PauseUpPressedRequested;

    // Pause 상태에서 아래 선택 이동 요청을 발행한다.
    public event Action PauseDownPressedRequested;

    // Pause 상태에서 선택 확정 요청을 발행한다.
    public event Action PauseSubmitPressedRequested;

    // Dialogue 상태에서 다음 대사 진행 및 선택 확정 요청을 발행한다.
    public event Action DialogueConfirmRequested;

    // Dialogue 상태에서 위 선택 이동 요청을 발행한다.
    public event Action DialogueUpRequested;

    // Dialogue 상태에서 아래 선택 이동 요청을 발행한다.
    public event Action DialogueDownRequested;

    // CutScene 상태에서 스킵 요청을 발행한다.
    public event Action CutsceneSkipRequested;

    private void Awake()
    {
        // 중복 매니저가 생성되면 새 오브젝트를 제거한다.
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // 최초 생성된 매니저를 전역 인스턴스로 고정한다.
        instance = this;
        DontDestroyOnLoad(gameObject);
        Initialize(keyBindingData);
    }

    private void Update()
    {
        switch (CurrentStateType)
        {
            case InputStateType.Gameplay:
                if (GameplayState != null && GameplayState.PausePressed)
                {
                    PauseToggleRequested?.Invoke();
                }
                break;

            case InputStateType.Pause:
                if (PauseState == null) break;
                else if (PauseState.ResumePressed)
                {
                    PauseToggleRequested?.Invoke();
                }

                bool upPressed = PauseState.UpPressed;
                bool downPressed = PauseState.DownPressed;
                bool pauseSubmitPressed = PauseState.SubmitPressed;

                if (upPressed)
                {
                    PauseUpPressedRequested?.Invoke();
                }
                if (downPressed)
                {
                    PauseDownPressedRequested?.Invoke();
                }
                if (pauseSubmitPressed)
                {
                    PauseSubmitPressedRequested?.Invoke();
                }
                break;

            case InputStateType.Dialogue:
                if (DialogueState == null) break;

                bool confirmPressed = DialogueState.NextPressed || DialogueState.SubmitPressed;

                if (confirmPressed)
                {
                    DialogueConfirmRequested?.Invoke();
                }


                if (DialogueState.UpPressed)
                {
                    DialogueUpRequested?.Invoke();
                }

                if (DialogueState.DownPressed)
                {
                    DialogueDownRequested?.Invoke();
                }
                break;

            case InputStateType.CutScene:
                if (CutsceneState != null && CutsceneState.SkipPressed)
                {
                    CutsceneSkipRequested?.Invoke();
                }
                break;
        }
    }

    // 키 바인딩 데이터로 각 입력 상태 인스턴스를 생성한다.
    public void Initialize(KeyBindingData data)
    {
        states.Clear();
        keyBindingData = data;

        // 키 바인딩 데이터가 없으면 상태를 초기화하지 않는다.
        if (keyBindingData == null)
        {
            GameplayState = null;
            PauseState = null;
            DialogueState = null;
            CutsceneState = null;
            CurrentState = null;
            Debug.LogError("InputStateManager.Initialize failed: KeyBindingData is null.");
            return;
        }

        GameplayState = new GameplayInputState(keyBindingData);
        PauseState = new PauseInputState(keyBindingData);
        DialogueState = new DialogueInputState(keyBindingData);
        CutsceneState = new CutsceneInputState(keyBindingData);

        states[InputStateType.Gameplay] = GameplayState;
        states[InputStateType.Pause] = PauseState;
        states[InputStateType.Dialogue] = DialogueState;
        states[InputStateType.CutScene] = CutsceneState;

        // 기본 입력 상태는 Gameplay로 시작한다.
        ChangeState(InputStateType.Gameplay);
    }

    // 활성 입력 상태를 지정된 상태로 교체한다.
    public void ChangeState(InputStateType nextState)
    {
        if (!states.TryGetValue(nextState, out IInputState next))
        {
            Debug.LogWarning($"Input state is not registered: {nextState}");
            return;
        }

        InputStateType previousState = CurrentStateType;
        CurrentStateType = nextState;
        CurrentState = next;

        // Dialogue/CutScene/Pause -> Gameplay 전환 시 같은 프레임 입력 누수를 막는다.
        if (nextState == InputStateType.Gameplay && previousState != InputStateType.Gameplay && GameplayState != null)
        {
            GameplayState.BlockInputThisFrame();
        }
    }

    // 현재 상태 타입이 지정 상태와 같은지 확인한다.
    public bool IsState(InputStateType state)
    {
        return CurrentStateType == state;
    }
}
