using System.Linq;
using UnityEngine;

public enum QuestStage
{
    Locked,
    Available,
    Failed,
    Completed,
    InProgress
}

public class LoadCheckNPC_ByQuest : MonoBehaviour
{
    [Header("대화 대상")]
    [SerializeField] private NPCDialogue targetNPC;

    [Header("대상 퀘스트")]
    [SerializeField] private QuestDefinition quest;   // SO 참조

    [Header("이 스크립트가 반응할 퀘스트 단계")]
    [SerializeField] private QuestStage[] targetStages;

    [Header("선택: 시작 시 1회 실행할 이벤트(필요 시만 사용)")]
    [SerializeField] private UnityEngine.Events.UnityEvent[] onStartEvents;

    private void Start()
    {
        // 필요 시: 씬 로드 직후 1회 실행
        if (onStartEvents != null && onStartEvents.Length > 0)
        {
            for (int i = 0; i < onStartEvents.Length; i++)
                onStartEvents[i]?.Invoke();
        }
    }

    public void Interactive() => Invoke(nameof(AutoInteractive), 0.55f);

    private void AutoInteractive()
    {
        if (!targetNPC || quest == null || QuestJournal.Instance == null) return;

        var stage = GetQuestStage(quest.QuestId);
        bool isTarget = targetStages != null && targetStages.Any(s => s == stage);

        if (isTarget)
            targetNPC.Interactive();
    }

    // ─────────────────────────────────────────────
    // 헬퍼: QuestStatus → QuestStage 매핑
    // Locked/Available ⇒ BeforeStart, InProgress ⇒ InProgress, Completed/Failed ⇒ AfterComplete
    // ─────────────────────────────────────────────
    private QuestStage GetQuestStage(string questId)
    {
        var st = QuestJournal.Instance.ComputeStatus(questId);
        switch (st)
        {
            case QuestStatus.InProgress: return QuestStage.InProgress;
            case QuestStatus.Completed: return QuestStage.Completed;
            case QuestStatus.Failed: return QuestStage.Failed;
            case QuestStatus.Available: return QuestStage.Available;
            case QuestStatus.Locked: return QuestStage.Locked;
            default: return QuestStage.Available;
        }
    }

    // ─────────────────────────────────────────────
    // (선택) 디버그/연출용 퀘스트 조작 헬퍼
    // ─────────────────────────────────────────────
    public void AcceptQuest()
    {
        if (quest != null) QuestJournal.Instance?.Accept(quest.QuestId);
    }

    public void TryCompleteQuest()
    {
        if (quest != null) QuestJournal.Instance?.TryComplete(quest.QuestId);
    }

    public void FailQuest()
    {
        if (quest != null) QuestJournal.Instance?.Fail(quest.QuestId);
    }
}
