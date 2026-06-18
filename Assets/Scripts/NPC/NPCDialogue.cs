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

// 현재 node에 연결된 Scene 이벤트를 정의한다.
// OnEnter: 해당 node가 시작될 때 한 번 호출된다.
// OnEnd: 해당 node의 모든 텍스트 출력이 끝난 직후 호출된다.
// OptionEvents: 선택지 확정 시 선택된 index에 맞춰 호출된다.
[Serializable]
public class NodeEvent
{
    [field: SerializeField] public DialogueNodeData Node { get; private set; }
    [field: SerializeField] public UnityEvent OnEnter { get; private set; }
    [field: SerializeField] public UnityEvent OnEnd { get; private set; }
    [field: SerializeField] public UnityEvent[] OptionEvents { get; private set; }
}


// NPC 대화와 상호작용을 처리한다.
public class NPCDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform textPosition;

    [SerializeField] private NPCDialogueData dialogueData;

    [SerializeField] private NodeEvent[] nodeEvents;

    [field: SerializeField] public NPCId NPCId { get; private set; }
    [field: SerializeField] public Transform InteractivePos { get; set; }

    private readonly Dictionary<DialogueNodeData, NodeEvent> eventMap = new();
    private readonly NpcDialogueResolver resolver = new();

    private static readonly int IsTalkHash = Animator.StringToHash(AnimatorParams.IsTalk);

    private bool hasTalkParameter;

    public event Action OnDialogueSignal;

    [SerializeField] private DialogueNodeData entry;
    [SerializeField] private Animator npcAnimator;
    public void SetEntry(DialogueNodeData node) => entry = node;

    protected virtual void Awake()
    {
        npcAnimator = GetComponent<Animator>();
        hasTalkParameter = HasAnimatorParameter(npcAnimator, IsTalkHash);

        if (!ValidateDialogueData()) return;

        BuildEventMap();
    }

    private bool HasAnimatorParameter(Animator animator, int parameterHash)
    {
        if (animator == null) return false;

        AnimatorControllerParameter[] parameters = animator.parameters;
        if (parameters == null || parameters.Length == 0) return false;

        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].nameHash == parameterHash) return true;
        }

        return false;
    }
    protected virtual bool ValidateDialogueData()
    {
        if (dialogueData == null)
        {
            Debug.LogError($"[NPCDialogue] DialogueData가 null 입니다. npcId = {NPCId}", this);
            
            return false;
        }

        if (dialogueData.NpcId != NPCId)
        {
            Debug.LogError(
            $"[NPCDialogue] NPCId와 DialogueData.NpcId가 일치하지 않습니다. " +
            $"npcId: {NPCId}, dataNpcId: {dialogueData.NpcId}", this);
            return false;
        }

        return true;
    }

    private void BuildEventMap()
    {
        eventMap.Clear();

        if (nodeEvents == null || nodeEvents.Length == 0)
        {
            return;
        }

        for (int i = 0; i < nodeEvents.Length; i++)
        {
            NodeEvent nodeEvent = nodeEvents[i];
            if (nodeEvent == null)
            {
                Debug.LogWarning($"[NPCDialogue] nodeEvents[{i}] 가 null 입니다.", this);
                continue;
            }

            DialogueNodeData node = nodeEvent.Node;
            if (node == null)
            {
                Debug.LogWarning($"[NPCDialogue] nodeEvents[{i}] 의 node가 null 입니다.", this);
                continue;
            }

            if (dialogueData != null && !dialogueData.ContainsNode(node))
            {
                Debug.LogWarning($"[NPCDialogue] Dialogue에 등록되지 않은 NodeEvent 입니다." +
                    $"npcId : {NPCId}, node : {node.name}" , this);
                continue;
            }

            if (eventMap.ContainsKey(nodeEvent.Node))
            {
                Debug.LogWarning($"[NPCDialogue] 중복 노드 이벤트 등록 : {nodeEvent.Node.name}", this);
                continue;
            }

            ValidateOptionEventCount(node, nodeEvent, i);

            eventMap.Add(nodeEvent.Node, nodeEvent);
        }
    }

    private void ValidateOptionEventCount(DialogueNodeData node, NodeEvent nodeEvent, int index)
    {
        int optionCount = node.options != null ? node.options.Length : 0;
        int optionEventCount = nodeEvent.OptionEvents != null ? nodeEvent.OptionEvents.Length : 0;

        if (optionEventCount == 0) return;

        if (optionEventCount != optionCount)
        {
            Debug.LogWarning($"[NPCDialogue] OptionEvents 개수와 Options 개수가 다릅니다. " +
                $"npcId : {NPCId}, nodeEvents[{index}], node: {node.name}" +
                $"options: {optionCount}, optionEvents: {optionEventCount}", this);
        }
    }
    private DialogueNodeData ConsumeEntry()
    {
        DialogueNodeData consumedEntry = entry;
        entry = null;

        return consumedEntry;
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

    private void DialogueAnim(NPCId npcId, bool isStart)
    {
        if (npcId != NPCId) return;
        if (npcAnimator == null) return;
        if (!hasTalkParameter) return;

        npcAnimator.SetBool(IsTalkHash, isStart);
    }

    public virtual void Interactive(PlayerBase _ = null)
    {
        if (UIManager.Instance != null && UIManager.Instance.IsSceneTransitioning)
        {
            Debug.LogWarning("[NPCDialogue] 씬 전환 중에는 대화를 시작할 수 없습니다.", this);
            return;
        }

        if (dialogueData == null)
        {
            Debug.LogError($"[NPCDialogue] DialogueData 가 없습니다. npcId : {NPCId}");
            return;
        }

        DialogueNodeData startEntry = ConsumeEntry();

        if (!resolver.TryResolve(dialogueData, NPCId, startEntry, out DialogueNodeData start)) return;

        DialogueManager dialogueManager = DialogueManager.Instance;
        if (dialogueManager == null)
        {
            Debug.LogError("[NPCDialogue] NPCDialogueManager 가 null 입니다.", this);
            return;
        }

        dialogueManager.StartDialogue(
            start,
            GetOptionEvents(start),
            textPosition,
            NPCId,
            this
            );
    }

    public void ViewNextNode() => StartCoroutine(NextNodeWaitFrame());

    private IEnumerator NextNodeWaitFrame()
    {
        yield return new WaitForEndOfFrame();

        Interactive();
    }

    protected void RaiseDialogueSignal() => OnDialogueSignal?.Invoke();

    // 외부 시스템이 특정 노드를 직접 시작할 때 사용한다.
    public void StartDialogueWithNode(DialogueNodeData node, UnityEvent[] overrideOptionEvents = null)
    {
        if (UIManager.Instance != null && UIManager.Instance.IsSceneTransitioning)
        {
            Debug.LogWarning("[NPCDialogue] 씬 전환 중에는 대화를 시작할 수 없습니다.", this);
            return;
        }

        if (node == null)
        {
            Debug.LogWarning("[NPCDialogue] StartDialogueWithNode: node == null", this);
            return;
        }

        DialogueManager dialogueManager = DialogueManager.Instance;
        if (dialogueManager == null)
        {
            Debug.LogError("[NPCDialogue] NPCDialogueManager 가 null 입니다.", this);
            return;
        }

        dialogueManager.StartDialogue(
            node,
            overrideOptionEvents ?? GetOptionEvents(node),
            textPosition,
            NPCId,
            this
        );
    }

    // DialogueManager가 노드 진입 시 호출하는 훅이다.
    public void InvokeOnEnter(DialogueNodeData node)
    {
        if (node != null && eventMap.TryGetValue(node, out NodeEvent nodeEvent))
        {
            nodeEvent.OnEnter?.Invoke();
        }
    }

    // DialogueManager가 노드 종료 시 호출하는 훅이다.
    public void InvokeOnEnd(DialogueNodeData node)
    {
        if (node != null && eventMap.TryGetValue(node, out NodeEvent nodeEvent))
        {
            nodeEvent.OnEnd?.Invoke();
        }
    }
    public UnityEvent[] GetOptionEvents(DialogueNodeData node)
    {
        if (node != null && eventMap.TryGetValue(node, out NodeEvent nodeEvent))
        {
            return nodeEvent.OptionEvents;
        }

        return null;
    }

    // 상태 변경 헬퍼를 제공한다.
    public void SetState(NPCState state)
    {
        NPCStateManager npcStateManager = NPCStateManager.Instance;

        if (npcStateManager == null)
        {
            Debug.LogWarning("[NPCDialogue] NPCStateManager 가 null 입니다.", this);
            return;
        }
        npcStateManager.SetState(NPCId, state);
    }
    public void SetStateByInt(int value)
    {
        NPCStateManager npcStateManager = NPCStateManager.Instance;

        if (npcStateManager == null)
        {
            Debug.LogWarning("[NPCDialogue] NPCStateManager 가 null 입니다.", this);
            return;
        }

        npcStateManager.SetState(NPCId, (NPCState)value);
    }
    public void SetFirstMeet() => SetState(NPCState.FirstMeet);
    public void SetInProgress() => SetState(NPCState.InProgress);
    public void SetCompleted() => SetState(NPCState.Completed);
    public void SetRepeat() => SetState(NPCState.Repeat);
    public void SetFailed() => SetState(NPCState.Failed);
}
