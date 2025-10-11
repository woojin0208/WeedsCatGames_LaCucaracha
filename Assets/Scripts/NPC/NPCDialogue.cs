using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum NPCState
{
    Completed = 4,
    Failed = 3,
    InProgress = 2,
    Repeat = 1,
    FirstMeet = 0
}

[System.Serializable]
public struct NodeEvent
{
    public DialogueNodeData node;
    public UnityEvent onEnter;
    public UnityEvent onEnd;

    // 추가: 이 노드에서의 옵션 이벤트들(옵션 수/순서와 매칭)
    public UnityEvent[] optionEvents;
}

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform textPosition;

    // (선택) 스타트 인덱스로 쓰고 싶으면 유지
    [SerializeField] private DialogueNodeData[] dialogueNodeData;

    [Header("Node ↔ Event Mapping (노드 참조로 직접 매칭)")]
    [SerializeField] private NodeEvent[] nodeEvents;

    [SerializeField] private UnityEvent[] optionEvents;

    [field: SerializeField] public NPCId NPCId { get; private set; }
    [field: SerializeField] public Transform InteractivePos { get; set; }


    private readonly Dictionary<DialogueNodeData, NodeEvent> _eventMap = new();

    public event Action OnDialogueSignal;

    protected virtual void Awake()
    {
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

    public virtual void Interactive(EntityBase _ = null)
    {
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
    /// <summary>외부(예: GuardedEntrance)에서 특정 노드로 강제 시작</summary>
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

    // ---- DialogueManager가 호출하는 훅 ----
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
        return optionEvents; // 등록 안 했으면 공용 배열 사용(선택)
    }

    // ---- 상태 헬퍼 ----
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
