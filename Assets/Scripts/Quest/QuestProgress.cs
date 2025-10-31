using System;
using UnityEngine;

public enum QuestStatus
{
    Locked,
    Available,
    InProgress,
    Completed,
    Failed
}

[Serializable]
public class QuestProgress
{
    // 필드
    public string QuestId { private set; get; }
    public QuestStatus Status { private set; get; }
    public int[] CurrentCounts { private set; get; }

    // 초기화
    public static QuestProgress Start(QuestDefinition def)
    {
        if (def == null) throw new ArgumentNullException(nameof(def));

        return new QuestProgress
        {
            QuestId = def.QuestId,
            Status = QuestStatus.InProgress,
            CurrentCounts = new int[def.ObjectiveCount]
        };
    }

    /// <summary>해당 인덱스 목표가 요구 수량(require) 이상 채워졌는지</summary>
    public bool IsObjectiveCompleted(int idx, int require)
    {
        if (CurrentCounts == null || idx < 0 || idx >= CurrentCounts.Length) return false;
        if (require < 1) require = 1;
        return CurrentCounts[idx] >= require;
    }

    /// <summary>모든 목표가 완료되었는지(정의 기반)</summary>
    public bool AllObjectivesCompleted(QuestDefinition def)
    {
        if (def == null || def.ObjectiveCount == 0) return false;
        if (CurrentCounts == null || CurrentCounts.Length < def.ObjectiveCount) return false;

        for (int i = 0; i < def.ObjectiveCount; i++)
        {
            var req = def.Objectives[i].requireCount < 1 ? 1 : def.Objectives[i].requireCount;
            if (CurrentCounts[i] < req) return false;
        }
        return true;
    }

    // ── 선택 유틸(있으면 편한 정도) ───────────────────────────────
    /// <summary>진행 수치 갱신(클램프 포함)</summary>
    public void SetCount(int idx, int value, QuestDefinition def)
    {
        if (def == null) return;
        EnsureArray(def.ObjectiveCount);
        if ((uint)idx >= (uint)CurrentCounts.Length) return;

        int require = def.Objectives[idx].requireCount < 1 ? 1 : def.Objectives[idx].requireCount;
        CurrentCounts[idx] = Mathf.Clamp(value, 0, require);
    }

    /// <summary>진행 수치 += add (클램프 포함)</summary>
    public void AddCount(int idx, int add, QuestDefinition def)
    {
        if (def == null) return;
        EnsureArray(def.ObjectiveCount);
        if ((uint)idx >= (uint)CurrentCounts.Length) return;

        int require = def.Objectives[idx].requireCount < 1 ? 1 : def.Objectives[idx].requireCount;
        int next = CurrentCounts[idx] + add;
        CurrentCounts[idx] = Mathf.Clamp(next, 0, require);
    }

    /// <summary>완료 마킹(검증은 외부에서 AllObjectivesCompleted로)</summary>
    public void MarkCompleted() => Status = QuestStatus.Completed;
    public void MarkFailed() => Status = QuestStatus.Failed;
    // ── 내부 ───────────────────────────────────────────────────────
    private void EnsureArray(int count)
    {
        if (CurrentCounts == null || CurrentCounts.Length != count)
            CurrentCounts = new int[count];
    }
}
