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

    // 다음 대사 표시를 기다리는 상태다.
    private bool waitingForLine;
    // 선택지 확정 입력을 기다리는 상태다.
    private bool waitingForOption;
    // 현재 선택 대기 중인 옵션 인덱스다.
    private int pendingOptionIndex = -1;

    // 노드 진입 이벤트를 한 번만 실행하기 위한 플래그다.
    private bool enteredNodeHandled;
    private UnityEvent[] optionEvents;

    public event Action<NPCId, bool> StartDialogueAction;

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

        manager.DialogueNextRequested -= HandleDialogueNextRequested;
        manager.DialogueSubmitRequested -= HandleDialogueSubmitRequested;
        manager.DialogueUpRequested -= HandleDialogueUpRequested;
        manager.DialogueDownRequested -= HandleDialogueDownRequested;

        if (!isSubscribe) return;

        manager.DialogueNextRequested += HandleDialogueNextRequested;
        manager.DialogueSubmitRequested += HandleDialogueSubmitRequested;
        manager.DialogueUpRequested += HandleDialogueUpRequested;
        manager.DialogueDownRequested += HandleDialogueDownRequested;
    }

    private void HandleDialogueNextRequested()
    {
        if (!waitingForLine) return;

        waitingForLine = false;
        ShowNextLine();
    }

    private void HandleDialogueSubmitRequested()
    {
        if (!waitingForOption) return;

        waitingForOption = false;
        SelectOption(pendingOptionIndex);
    }

    private void HandleDialogueUpRequested()
    {
        if (!waitingForOption || currentNode == null) return;

        pendingOptionIndex = Mathf.Max(0, pendingOptionIndex - 1);
        dialogueUI.HighlightOption(pendingOptionIndex);
    }

    private void HandleDialogueDownRequested()
    {
        if (!waitingForOption || currentNode == null || currentNode.options == null) return;

        pendingOptionIndex = Mathf.Min(currentNode.options.Length - 1, pendingOptionIndex + 1);
        dialogueUI.HighlightOption(pendingOptionIndex);
    }

    public void StartDialogue(DialogueNodeData dialogue, UnityEvent[] events,
                              Transform target, NPCId npcId, NPCDialogue hook)
    {
        SetInputEventSubscription(true);
        InputStateManager.Instance?.ChangeState(InputStateType.Dialogue);
        StartDialogueAction?.Invoke(npcId, true);
        currentNode = dialogue;
        optionEvents = events;
        currentNpcId = npcId;
        hookOwner = hook;

        optionEvents = hookOwner?.GetOptionEvents(currentNode) ?? events;

        lineIndex = 0;
        enteredNodeHandled = false;
        waitingForLine = waitingForOption = false;

        dialogueUI.Show(target);
        ShowNextLine();
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

        if (lineIndex < currentNode.texts.Length)
        {
            dialogueUI.ShowLine($"{currentNode.entityName}: {currentNode.texts[lineIndex]}");
            lineIndex++;
            waitingForLine = true;
            return;
        }

        hookOwner?.InvokeOnEnd(currentNode);

        var opts = currentNode.options;
        if (opts != null && opts.Length > 0)
        {
            if (opts.Length == 1)
            {
                optionEvents = hookOwner?.GetOptionEvents(currentNode) ?? optionEvents;
                var only = opts[0];
                dialogueUI.ShowOption(new List<string> { only.label });

                pendingOptionIndex = 0;
                waitingForOption = true;
                dialogueUI.HighlightOption(pendingOptionIndex);
            }
            else
            {
                var labels = new List<string>(opts.Length);
                for (int i = 0; i < opts.Length; i++) labels.Add(opts[i].label);

                pendingOptionIndex = 0;
                waitingForOption = true;
                dialogueUI.ShowOption(labels);
                dialogueUI.HighlightOption(pendingOptionIndex);
            }
        }
        else
        {
            EndDialogue();
        }
    }

    private void SelectOption(int idx)
    {
        var opts = currentNode.options;
        if (opts == null || idx < 0 || idx >= opts.Length)
        {
            EndDialogue();
            return;
        }

        if (optionEvents != null && idx < optionEvents.Length)
            optionEvents[idx]?.Invoke();

        currentNode = opts[idx].nextNode;
        lineIndex = 0;
        enteredNodeHandled = false;

        ShowNextLine();
    }

    private void EndDialogue()
    {
        StartDialogueAction?.Invoke(currentNpcId, false);
        dialogueUI.Hide();
        currentNode = null;
        waitingForLine = waitingForOption = false;
        hookOwner = null;
        InputStateManager.Instance?.ChangeState(InputStateType.Gameplay);
    }

    public void CloseDialogue()
    {
        dialogueUI.gameObject.SetActive(false);
        waitingForLine = false;
        waitingForOption = false;
        currentNode = null;
        hookOwner = null;
        InputStateManager.Instance?.ChangeState(InputStateType.Gameplay);
    }
}
