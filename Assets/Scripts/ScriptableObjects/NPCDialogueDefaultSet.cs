// ─────────────────────────────────────────────────────────────
// NPCDialogueDefaultSet.cs
// 역할: NPC 기본 대사 세트(슬롯형) + 기본 NodeEvent 매핑(SO)
// byState[(int)NPCState] : FirstMeet/Repeat/InProgress/Failed/Completed
// ─────────────────────────────────────────────────────────────
using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game/Dialogue/NPC Default Set", fileName = "NPCDialogueDefaultSet_")]
public class NPCDialogueDefaultSet : ScriptableObject
{
    [Header("Owner NPC")]
    [SerializeField] private NPCId npcId = default;
    public NPCId NpcId => npcId;

    [Header("Default Nodes by NPCState")]
    [Tooltip("인덱스 = (int)NPCState\n[0]=FirstMeet, [1]=Repeat, [2]=InProgress, [3]=Failed, [4]=Completed")]
    [SerializeField] private DialogueNodeData[] byState = new DialogueNodeData[STATE_COUNT];

    [Header("Optional: 기본 NodeEvent 매핑 (노드별 onEnter/onEnd/옵션 이벤트)")]
    [SerializeField] private NodeEvent[] defaultNodeEvents;

    private const int STATE_COUNT = 5; // FirstMeet/Repeat/InProgress/Failed/Completed

    /// <summary>해당 상태의 기본 시작 노드 반환(없으면 null)</summary>
    public DialogueNodeData GetNode(NPCState state)
    {
        int idx = (int)state;
        return (idx >= 0 && idx < byState.Length) ? byState[idx] : null;
    }

    /// <summary>기본 NodeEvent에서 특정 노드의 onEnter 호출</summary>
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

    /// <summary>기본 NodeEvent에서 특정 노드의 onEnd 호출</summary>
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

    /// <summary>기본 NodeEvent에서 특정 노드의 옵션 이벤트 배열을 얻음(없으면 null)</summary>
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
        // byState 길이 보정 (에디터 편의)
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
