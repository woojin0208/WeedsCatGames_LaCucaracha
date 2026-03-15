using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game/Dialogue/NPC Default Set", fileName = "NPCDialogueDefaultSet_")]
// NPC 기본 대화 세트를 정의한다.
public class NPCDialogueDefaultSet : ScriptableObject
{
    [Header("Owner NPC")]
    [SerializeField] private NPCId npcId = default;
    public NPCId NpcId => npcId;

    [Header("Default Nodes by NPCState")]
    [Tooltip("인덱스는 (int)NPCState와 대응한다.\n(int)NPCState\n[0]=FirstMeet, [1]=Repeat, [2]=InProgress, [3]=Failed, [4]=Completed")]
    [SerializeField] private DialogueNodeData[] byState = new DialogueNodeData[STATE_COUNT];

    [Header("Optional: 기본 NodeEvent 목록")]
    [Tooltip("각 노드별 onEnter, onEnd, 선택지 이벤트를 기본값으로 보관한다.")]
    [SerializeField] private NodeEvent[] defaultNodeEvents;

    private const int STATE_COUNT = 5; // FirstMeet/Repeat/InProgress/Failed/Completed

    // 전달한 상태에 대응하는 기본 대화 노드를 반환한다.
    public DialogueNodeData GetNode(NPCState state)
    {
        int idx = (int)state;
        return (idx >= 0 && idx < byState.Length) ? byState[idx] : null;
    }

    // 기본 NodeEvent 목록에서 지정한 노드의 onEnter 이벤트를 호출한다.
    public void InvokeDefaultOnEnter(DialogueNodeData node)
    {
        if (node == null || defaultNodeEvents == null) return;
        for (int i = 0; i < defaultNodeEvents.Length; i++)
        {
            if (defaultNodeEvents[i].node == node)
            {
                defaultNodeEvents[i].onEnter?.Invoke();
                return;
            }
        }
    }

    // 기본 NodeEvent 목록에서 지정한 노드의 onEnd 이벤트를 호출한다.
    public void InvokeDefaultOnEnd(DialogueNodeData node)
    {
        if (node == null || defaultNodeEvents == null) return;
        for (int i = 0; i < defaultNodeEvents.Length; i++)
        {
            if (defaultNodeEvents[i].node == node)
            {
                defaultNodeEvents[i].onEnd?.Invoke();
                return;
            }
        }
    }

    // 기본 NodeEvent 목록에서 지정한 노드의 선택지 이벤트 배열을 반환한다.
    public UnityEvent[] GetDefaultOptionEvents(DialogueNodeData node)
    {
        if (node == null || defaultNodeEvents == null) return null;
        for (int i = 0; i < defaultNodeEvents.Length; i++)
        {
            if (defaultNodeEvents[i].node == node)
                return defaultNodeEvents[i].optionEvents;
        }
        return null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 상태별 배열 길이를 고정해 인덱스 매핑이 깨지지 않도록 유지한다.
        if (byState == null || byState.Length != STATE_COUNT)
        {
            var old = byState ?? Array.Empty<DialogueNodeData>();
            var resized = new DialogueNodeData[STATE_COUNT];
            int copy = Mathf.Min(old.Length, resized.Length);
            for (int i = 0; i < copy; i++) resized[i] = old[i];
            byState = resized;
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}
