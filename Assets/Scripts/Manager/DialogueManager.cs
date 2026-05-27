using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 대화 진행과 선택지 입력을 관리한다.
public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;
    public static DialogueManager Instance => instance;

    [SerializeField] private DialogueUI dialogueUI;

    private DialogueNodeData currentNode;
    private NPCId currentNpcId;
    private NPCDialogue hookOwner;

    private int lineIndex;
    private int pendingOptionIndex = -1;

    // 다음 대사 표시를 기다리는 상태다.
    private bool waitingForLine;
    // 선택지 확정 입력을 기다리는 상태다.
    private bool waitingForOption;

    // 노드 진입 이벤트를 한 번만 실행하기 위한 플래그다.
    private bool enteredNodeHandled;

    // 현재 노드에 연결된 선택지 이벤트다.
    private UnityEvent[] optionEvents;

    public event Action<NPCId, bool> StartDialogueAction;

    private bool isDialogueRunning;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SetInputEventSubscription(true);
    }

    private void OnDisable()
    {
        SetInputEventSubscription(false);
    }

    private void SetInputEventSubscription(bool isSubscribe)
    {
        InputStateManager manager = InputStateManager.Instance;
        if (manager == null) return;

        manager.DialogueConfirmRequested -= HandleDialogueConfirmRequested;
        manager.DialogueUpRequested -= HandleDialogueUpRequested;
        manager.DialogueDownRequested -= HandleDialogueDownRequested;

        if (!isSubscribe) return;

        manager.DialogueConfirmRequested += HandleDialogueConfirmRequested;
        manager.DialogueUpRequested += HandleDialogueUpRequested;
        manager.DialogueDownRequested += HandleDialogueDownRequested;
    }

    private void HandleDialogueConfirmRequested()
    {
        if (waitingForLine)
        {
            waitingForLine = false;
            ShowNextLine();
            return;
        }

        if (waitingForOption)
        {
            waitingForOption = false;
            SelectOption(pendingOptionIndex);
            return;
        }
    }

    private void HandleDialogueUpRequested()
    {
        if (!waitingForOption || currentNode == null) return;

        DialogueOption[] options = currentNode.options;
        if (options == null || options.Length == 0) return;

        pendingOptionIndex = Mathf.Max(0, pendingOptionIndex - 1);
        dialogueUI?.HighlightOption(pendingOptionIndex);
    }

    private void HandleDialogueDownRequested()
    {
        if (!waitingForOption || currentNode == null) return;

        DialogueOption[] options = currentNode.options;
        if (options == null || options.Length == 0) return;

        pendingOptionIndex = Mathf.Min(options.Length - 1, pendingOptionIndex + 1);
        dialogueUI?.HighlightOption(pendingOptionIndex);
    }

    public void StartDialogue(DialogueNodeData dialogue, UnityEvent[] events, Transform target, NPCId npcId, NPCDialogue hook)
    {
        if (dialogue == null)
        {
            Debug.LogWarning("[DialogueManager] 시작할 DialogueNodeData 가 null 입니다.");
            return;
        }

        if (dialogueUI == null)
        {
            Debug.LogError("[DialogueManager] DialogueUI 가 null 입니다.");
            return;
        }

        if (isDialogueRunning) EndDialogue();

        SetInputEventSubscription(true);
        Time.timeScale = 0f;
        InputStateManager.Instance?.ChangeState(InputStateType.Dialogue);

        currentNode = dialogue;
        currentNpcId = npcId;
        hookOwner = hook;

        lineIndex = 0;
        pendingOptionIndex = -1;
        enteredNodeHandled = false;
        waitingForLine = false;
        waitingForOption = false;
        isDialogueRunning = true;

        optionEvents = ResolveOptionEvents(events);

        StartDialogueAction?.Invoke(currentNpcId, true);

        dialogueUI.Show(target);
        ShowNextLine();
    }

    private UnityEvent[] ResolveOptionEvents(UnityEvent[] fallbackEvents = null)
    {
        UnityEvent[] nodeEvents = hookOwner?.GetOptionEvents(currentNode);
        return nodeEvents ?? fallbackEvents;
    }

    private void HandleEnterActionsIfNeeded()
    {
        if (enteredNodeHandled || currentNode == null) return;

        hookOwner?.InvokeOnEnter(currentNode);
        enteredNodeHandled = true;
    }

    private void ShowNextLine()
    {
        if (currentNode == null)
        {
            EndDialogue();
            return;
        }

        HandleEnterActionsIfNeeded();

        string[] texts = currentNode.texts;
        if (texts != null && lineIndex < texts.Length)
        {
            dialogueUI.ShowLine(texts[lineIndex]);
            lineIndex++;
            waitingForLine = true;
            return;

        }

        // 모든 텍스트가 출력된 직후 실행한다.
        // 선택지가 있는 node라면 선택지 표시 전에 호출된다.
        hookOwner?.InvokeOnEnd(currentNode);

        DialogueOption[] options = currentNode.options;
        if (options == null || options.Length == 0)
        {
            EndDialogue();
            return;
        }

        ShowOptions(options);
    }

    private void ShowOptions(DialogueOption[] options)
    {
        optionEvents = ResolveOptionEvents(optionEvents);

        List<string> labels = new List<string>(options.Length);
        for (int i = 0; i < options.Length; i++)
        {
            labels.Add(options[i] != null ? options[i].Label : string.Empty);
        }

        pendingOptionIndex = 0;
        waitingForOption = true;

        dialogueUI.ShowOption(labels);
        dialogueUI.HighlightOption(pendingOptionIndex);
    }

    private void SelectOption(int index)
    {
        if (currentNode == null)
        {
            EndDialogue();
            return;
        }

        DialogueOption[] options = currentNode.options;
        if (options == null || index < 0 || index >= options.Length)
        {
            EndDialogue();
            return;
        }

        if (optionEvents != null && index < optionEvents.Length)
        {
            optionEvents[index]?.Invoke();
        }

        DialogueOption selectedOption = options[index];
        currentNode = selectedOption != null ? selectedOption.nextNode : null;

        lineIndex = 0;
        pendingOptionIndex = -1;
        enteredNodeHandled = false;
        waitingForLine = false;
        waitingForOption = false;

        optionEvents = ResolveOptionEvents();

        ShowNextLine();
    }

    private void EndDialogue()
    {
        if (!isDialogueRunning) return;

        isDialogueRunning = false;

        StartDialogueAction?.Invoke(currentNpcId, false);

        if (dialogueUI != null)
            dialogueUI.Hide();

        ClearRuntimeState();

        if (UIManager.Instance != null && UIManager.Instance.IsSceneTransitioning)
        {
            return;
        }

        Time.timeScale = 1f;

        InputStateManager inputStateManager = InputStateManager.Instance;
        if (inputStateManager != null && inputStateManager.IsState(InputStateType.Dialogue))
        {
            inputStateManager.ChangeState(InputStateType.Gameplay);
        }
    }

    public void CloseDialogue()
    {
        EndDialogue();
    }

    private void ClearRuntimeState()
    {
        currentNode = null;
        hookOwner = null;
        lineIndex = 0;
        pendingOptionIndex = -1;
        enteredNodeHandled = false;
        waitingForLine = false;
        waitingForOption = false;
        optionEvents = null;
    }
}
