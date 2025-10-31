// ─────────────────────────────────────────────────────────────
// QuestDialogueOverlay.cs
// 역할: 퀘스트별 “부분 패치(덧씌우기)” 모음 (SO)
// 적용 규칙:
//  - QuestJournal.IsOverlayActive(questId)가 true일 때만 후보
//  - 한 NPC의 특정 상태 슬롯만 있으면 그 슬롯만 교체(나머지는 기본 유지)
//  - 동시 후보 존재 시: QuestJournal.GetOverlaySituationScore(questId, npcId) 우선 → priority 높은 것
// ─────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class StatePatch
{
    public NPCState state;
    [Tooltip("이 상태에서 시작할 노드로 교체")]
    public DialogueNodeData nodeOverride;

    [Header("NodeEvent 덮어쓰기(옵션)")]
    [Tooltip("이 상태에서 사용할 NodeEvent들(노드 onEnter/onEnd/옵션 이벤트 매핑)")]
    public NodeEvent[] nodeEventsOverride;

    [Tooltip("true면 NodeEvent를 전부 이 배열로 교체 / false면 없으면 기본 유지")]
    public bool replaceAllNodeEvents = false;

    [Header("옵션 이벤트 덮어쓰기(옵션)")]
    [Tooltip("옵션 버튼별 이벤트(인덱스 매칭). 비어있으면 기본 유지")]
    public UnityEvent[] optionEventsOverride;
}

[Serializable]
public class NPCPatch
{
    public NPCId npcId;
    public List<StatePatch> states = new();

    public bool TryGet(NPCState state, out StatePatch patch)
    {
        if (states != null)
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i] != null && states[i].state == state)
                {
                    patch = states[i];
                    return true;
                }
            }
        }
        patch = null;
        return false;
    }
}

[CreateAssetMenu(menuName = "Game/Dialogue/Quest Dialogue Overlay", fileName = "QuestDialogueOverlay_")]
public class QuestDialogueOverlay : ScriptableObject
{
    [Header("Identification")]
    [SerializeField, Tooltip("이 오버레이가 적용될 퀘스트 ID(QuestDefinition.QuestId와 동일)")]
    private string questId;
    public string QuestId => questId;

    [SerializeField, Tooltip("동점일 때 높은 값이 우선")]
    private int priority = 0;
    public int Priority => priority;

    [Header("Patches")]
    [SerializeField, Tooltip("NPC별 상태 패치 목록")]
    private List<NPCPatch> patches = new();
    public IReadOnlyList<NPCPatch> Patches => patches;

    /// <summary>이 오버레이가 현재 “활성”인지: Journal 정책에 위임</summary>
    public bool IsActive()
        => QuestJournal.Instance != null && QuestJournal.Instance.IsOverlayActive(questId);

    /// <summary>이 오버레이가 특정 NPC/상태에 패치를 제공하는지</summary>
    public bool TryGetPatch(NPCId npcId, NPCState state, out StatePatch patch)
    {
        if (patches != null)
        {
            for (int i = 0; i < patches.Count; i++)
            {
                var npcPatch = patches[i];
                if (npcPatch == null) continue;
                if (npcPatch.npcId.Equals(npcId) && npcPatch.TryGet(state, out patch))
                    return true;
            }
        }
        patch = null;
        return false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(questId))
        {
            // 에디터 편의상 파일명 기반 기본값
            questId = name.Replace("QuestDialogueOverlay_", string.Empty);
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}
