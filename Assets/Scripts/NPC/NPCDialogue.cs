using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// NPC의 상태를 정의한다.
public enum NPCState
{
    Completed = 4,
    Failed = 3,
    InProgress = 2,
    Repeat = 1,
    FirstMeet = 0
}

// 현재 node를 정의한다.
[System.Serializable]
public struct NodeEvent
{
    public DialogueNodeData node;
    public UnityEvent onEnter;
    public UnityEvent onEnd;

    public UnityEvent[] optionEvents;
}

// NPC 대화와 상호작용을 처리한다.
public class NPCDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform textPosition;

    [SerializeField] private DialogueNodeData[] dialogueNodeData;

    [Header("Node ↔ Event Mapping (노드 참조로 직접 매칭)")]
    [SerializeField] private NodeEvent[] nodeEvents;

    [SerializeField] private UnityEvent[] optionEvents;

    [field: SerializeField] public NPCId NPCId { get; private set; }
    [field: SerializeField] public Transform InteractivePos { get; set; }

    private readonly Dictionary<DialogueNodeData, NodeEvent> _eventMap = new();

    public event Action OnDialogueSignal;

    [SerializeField] private DialogueNodeData entry;
    [SerializeField] private Animator npcAnimator;
    public void SetEntry(DialogueNodeData node) => entry = node;

    protected virtual void Awake()
    {
        npcAnimator = GetComponent<Animator>();

        _eventMap.Clear();
        if (nodeEvents != null)
        {
            foreach (var ne in nodeEvents)
            {
                if (ne.node == null) continue;
                if (_eventMap.ContainsKey(ne.node))
                {
                    Debug.LogWarning($"[NPCDialogue] 중복 노드 등록: {ne.node.name}", this);
                    continue;
                }
                _eventMap.Add(ne.node, ne);
            }
        }
    }

    private void OnEnable()
    {
        SetDialogueEventSubscription(true);
    }

    private void OnDisable()
    {
        SetDialogueEventSubscription(false);
    }

    private void SetDialogueEventSubscription(bool isSubscribe)
    {
        DialogueManager manager = DialogueManager.Instance;
        if (manager == null) return;

        manager.StartDialogueAction -= DialogueAnim;
        if (isSubscribe)
        {
            manager.StartDialogueAction += DialogueAnim;
        }
    }

    private void DialogueAnim(NPCId npcID, bool isStart)
    {
        if (npcID == this.NPCId)
        {
            if (isStart) npcAnimator?.SetBool("IsTalk", true);
            else npcAnimator?.SetBool("IsTalk", false);
        }
    }
    public virtual void Interactive(PlayerBase _ = null)
    {
        if (entry != null)
        {
            DialogueManager.Instance.StartDialogue(
                entry,
                optionEvents,
                textPosition,
                NPCId,
                this
            );

            // 일회성 시작 노드는 한 번 사용 후 비운다.
            entry = null;
            return;
        }

        var state = NPCStateManager.Instance.GetState(NPCId);
        Debug.Log($"[NPCDialogue] Start by state = {state}");

        int idx = (int)state;

        DialogueNodeData start =
            (dialogueNodeData != null && idx >= 0 && idx < dialogueNodeData.Length)
            ? dialogueNodeData[idx]
            : null;

        if (start == null)
        {
            Debug.LogWarning($"[NPCDialogue] 시작 노드가 비었습니다. state={state}, idx={idx}", this);
            return;
        }

        DialogueManager.Instance.StartDialogue(
            start,
            optionEvents,
            textPosition,
            NPCId,
            this
        );
    }
    public void ViewNextNode()
    {
        StartCoroutine(NextNodeWaitFrame());
    }
    private IEnumerator NextNodeWaitFrame()
    {
        yield return new WaitForEndOfFrame();

        Interactive();
    }
    protected void RaiseDialogueSignal()
    {
        OnDialogueSignal?.Invoke(); Debug.Log("odk");
    }

    // 외부 시스템이 특정 노드를 직접 시작할 때 사용한다.
    public void StartDialogueWithNode(DialogueNodeData node, UnityEvent[] overrideOptionEvents = null)
    {
        if (node == null)
        {
            Debug.LogWarning("[NPCDialogue] StartDialogueWithNode: node == null", this);
            return;
        }

        DialogueManager.Instance.StartDialogue(
            node,
            overrideOptionEvents ?? optionEvents,
            textPosition,
            NPCId,
            this
        );
    }

    // DialogueManager가 노드 진입 시 호출하는 훅이다.
    public void InvokeOnEnter(DialogueNodeData node)
    {
        if (node != null && _eventMap.TryGetValue(node, out var ev))
        {
            ev.onEnter?.Invoke();
        }
        else
        {
            Debug.LogWarning($"[NPCDialogue] OnEnter 매핑 없음: {node?.name}", this);
        }
    }

    // DialogueManager가 노드 종료 시 호출하는 훅이다.
    public void InvokeOnEnd(DialogueNodeData node)
    {
        if (node != null && _eventMap.TryGetValue(node, out var ev))
        {
            ev.onEnd?.Invoke();
        }
        else
        {
            Debug.LogWarning($"[NPCDialogue] OnEnd 매핑 없음: {node?.name}", this);
        }
    }
    public UnityEvent[] GetOptionEvents(DialogueNodeData node)
    {
        if (node != null)
        {
            for (int i = 0; i < nodeEvents.Length; i++)
                if (nodeEvents[i].node == node)
                    return nodeEvents[i].optionEvents;
        }
        // 노드별 등록이 없으면 공용 선택지 이벤트 배열을 사용한다.
        return optionEvents;
    }

    // 상태 변경 헬퍼를 제공한다.
    public void SetState(NPCState s)
        => NPCStateManager.Instance.SetState(NPCId, s);

    public void SetStateByInt(int v)
        => NPCStateManager.Instance.SetState(NPCId, (NPCState)v);

    public void SetFirstMeet() => SetState(NPCState.FirstMeet);
    public void SetInProgress() => SetState(NPCState.InProgress);
    public void SetCompleted() => SetState(NPCState.Completed);
    public void SetRepeat() => SetState(NPCState.Repeat);
    public void SetFailed() => SetState(NPCState.Failed);
}
