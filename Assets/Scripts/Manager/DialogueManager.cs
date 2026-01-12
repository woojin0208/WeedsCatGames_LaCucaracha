using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;
    public static DialogueManager Instance => instance;

    [SerializeField] private DialogueUI dialogueUI;

    private DialogueNodeData currentNode;
    private NPCId currentNpcId;
    private NPCDialogue hookOwner;
    private int lineIndex;

    private bool waitingForLine;         // Space로 다음 대사
    private bool waitingForOption;       // Space/Enter로 옵션 선택
    private int pendingOptionIndex = -1; // 선택 대기 중인 옵션 인덱스

    private bool enteredNodeHandled;     // 노드별 onEnter 1회 처리
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

    private void Update()
    {
        if (waitingForLine && Input.GetKeyUp(KeyCode.Space))
        {
            waitingForLine = false;
            ShowNextLine();
            return;
        }
        else if (waitingForOption && (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return)))
        {
            waitingForOption = false;
            SelectOption(pendingOptionIndex);
            return;
        }

        // ↑/↓ 키로 옵션 이동
        if (waitingForOption && Input.GetKeyDown(KeyCode.UpArrow))
        {
            pendingOptionIndex = Mathf.Max(0, pendingOptionIndex - 1);
            dialogueUI.HighlightOption(pendingOptionIndex);
        }
        if (waitingForOption && Input.GetKeyDown(KeyCode.DownArrow))
        {
            pendingOptionIndex = Mathf.Min(currentNode.options.Length - 1, pendingOptionIndex + 1);
            dialogueUI.HighlightOption(pendingOptionIndex);
        }
    }

    public void StartDialogue(DialogueNodeData dialogue, UnityEvent[] events,
                              Transform target, NPCId npcId, NPCDialogue hook)
    {
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

        // 노드 OnEnter 이벤트
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

        // 노드 OnEnd 이벤트
        hookOwner?.InvokeOnEnd(currentNode);

        var opts = currentNode.options;
        if (opts != null && opts.Length > 0)
        {
            if (opts.Length == 1)
            {
                optionEvents = hookOwner?.GetOptionEvents(currentNode) ?? optionEvents;
                // 옵션이 1개일 때 → Player 대사처럼 버튼 1개 표시
                var only = opts[0];
                dialogueUI.ShowOption(new List<string> { only.label }, idx => SelectOption(idx));

                pendingOptionIndex = 0;
                waitingForOption = true; // Space/Enter로 선택 가능
            }
            else
            {
                // 옵션 여러 개
                var labels = new List<string>(opts.Length);
                for (int i = 0; i < opts.Length; i++) labels.Add(opts[i].label);

                pendingOptionIndex = 0;
                dialogueUI.ShowOption(labels, idx => SelectOption(idx));
                dialogueUI.HighlightOption(pendingOptionIndex);
            }
        }
        else
        {
            // 옵션 없으면 그냥 끝
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

        // 옵션 이벤트 실행
        if (optionEvents != null && idx < optionEvents.Length)
            optionEvents[idx]?.Invoke();

        // 다음 노드로 진행
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
    }

    public void CloseDialogue()
    {
        dialogueUI.gameObject.SetActive(false);

    }
}