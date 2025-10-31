using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ObjectiveType { TalkTo, Kill, Collect, GoTo }

[Serializable]
public class QuestObjectiveDef
{
    [Tooltip("목표 타입 (대화/처치/수집/이동 등)")]
    public ObjectiveType type;

    [Tooltip("타겟 식별자(NPCId/EnemyId/ItemId/AreaId 등 문자열 ID)")]
    public string targetId;

    [Min(1), Tooltip("필요 수량(토크/이동은 보통 1)")]
    public int requireCount = 1;

    [Tooltip("UI 표시용 간단 설명")]
    public string note;
}

[CreateAssetMenu(menuName = "Game/Quest", fileName = "Quest_")]
public class QuestDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField, Tooltip("퀘스트 고유 ID")]
    private string questId;
    public string QuestId => questId;

    [SerializeField, Tooltip("퀘스트 제목")]
    private string title;
    public string Title => title;

    [TextArea(3, 6), SerializeField, Tooltip("퀘스트 설명(요약/플레이어 가이드)")]
    private string description;
    public string Description => description;

    [Header("NPC / Flow")]

    [SerializeField, Tooltip("퀘스트 제공 NPC")]
    private NPCId acceptFrom = default;
    public NPCId AcceptFrom => acceptFrom;

    [SerializeField, Tooltip("퀘스트 완료 NPC")]
    private NPCId turnInTo = default;
    public NPCId TurnInTo => turnInTo;

    [Header("Prerequisites")]
    [SerializeField, Tooltip("완료되어 있어야 하는 선행 퀘스트 ID 목록")]
    private QuestDefinition[] requiredQuestCompleted;
    public IReadOnlyList<QuestDefinition> RequiredQuestCompleted => requiredQuestCompleted;

    [Header("Objectives")]
    [SerializeField, Tooltip("이 퀘스트의 목표 정의")]
    private List<QuestObjectiveDef> objectives = new();
    public IReadOnlyList<QuestObjectiveDef> Objectives => objectives;

    [Header("Hooks(액션)")]
    [SerializeField] private List<QuestAction> onAcceptActions;
    [SerializeField] private List<QuestAction> onCompleteActions;
    [SerializeField] private List<QuestAction> onFailActions;

    public IReadOnlyList<QuestAction> OnAcceptActions => onAcceptActions;
    public IReadOnlyList<QuestAction> OnCompleteActions => onCompleteActions;
    public IReadOnlyList<QuestAction> OnFailActions => onFailActions;
    /*
    [Header("Hooks(옵션)")]
    public UnityEvent onAccept;
    public UnityEvent onComplete;
    public UnityEvent onFail;
    */

    // ──────────────────────────────────────────────────────────────
    // 편의 함수 (런타임/툴 공용으로 쓰기 좋게 일부 제공)
    // ──────────────────────────────────────────────────────────────

    /// <summary>수락 담당 NPC 반환
    public NPCId GetAcceptNpc() => acceptFrom;

    /// <summary>턴인 담당 NPC 반환
    public NPCId GetTurnInNpc() => turnInTo;

    /// <summary>목표 총 개수</summary>
    public int ObjectiveCount => objectives?.Count ?? 0;

    /// <summary>유효성 간단 체크(에디터/런타임 공용)</summary>
    public bool IsValid(out string reason)
    {
        if (string.IsNullOrWhiteSpace(questId)) { reason = "questId가 비어 있습니다."; return false; }
        if (string.IsNullOrWhiteSpace(title)) { reason = "title이 비어 있습니다."; return false; }
        if (objectives == null || objectives.Count == 0)
        {
            reason = "objectives가 비어 있습니다.";
            return false;
        }
        foreach (var o in objectives)
        {
            if (o == null) { reason = "objectives 중 null 항목이 있습니다."; return false; }
            if (o.requireCount < 1) { reason = "objective.requireCount는 1 이상이어야 합니다."; return false; }
            if (string.IsNullOrWhiteSpace(o.targetId) && (o.type != ObjectiveType.GoTo && o.type != ObjectiveType.TalkTo))
            {
                reason = "일부 objective의 targetId가 비어 있습니다.";
                return false;
            }
        }
        reason = null; return true;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // questId 자동 보정: 비어 있으면 에셋명+짧은 GUID
        if (string.IsNullOrWhiteSpace(questId))
        {
            questId = $"{name}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            UnityEditor.EditorUtility.SetDirty(this);
        }

        // requireCount 최소 1 강제
        if (objectives != null)
        {
            foreach (var o in objectives)
            {
                if (o != null && o.requireCount < 1) o.requireCount = 1;
            }
        }
    }

    [ContextMenu("Regenerate QuestId (GUID suffix)")]
    private void RegenerateId()
    {
        questId = $"{name}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
