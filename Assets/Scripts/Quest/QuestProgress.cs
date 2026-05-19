using System;
using System.Collections.Generic;
using UnityEngine;

public enum QuestState
{
    // 아직 시작되지 않은 퀘스트 상태다.
    NotStarted,
    // 퀘스트가 진행 중이며 목표 진행도를 갱신할 수 있는 상태다.
    InProgress,
    // 퀘스트 목표는 완료됐지만 최종 보상은 아직 수령하지 않았을 수 있는 상태다.
    Completed,
    // 퀘스트 완료 후 보상 수령까지 끝난 상태다.
    Rewarded,
    // 일반적인 흐름으로 더 이상 완료할 수 없는 상태다.
    Failed
}

[Serializable]
// 단일 퀘스트의 런타임 진행 상태를 저장한다.
// 조건 판정과 보상 실행은 담당하지 않고 저장 가능한 상태만 보유한다.
public class QuestProgress
{
    public string QuestId { get; private set; }
    public QuestState State { get; private set; }

    // 목표 진행도는 objectiveId 기준으로 저장해 씬 오브젝트 배치에 의존하지 않도록 한다.
    private readonly Dictionary<string, int> objectiveCounters = new();
    // 다중 목표 퀘스트를 위해 완료된 목표 목록은 QuestState와 별도로 관리한다.
    private readonly HashSet<string> completeObjectiveIds = new();

    public IReadOnlyDictionary<string, int> ObjectiveCounters => objectiveCounters;
    public IReadOnlyCollection<string> CompletedObjectiveIds => completeObjectiveIds;

    public QuestProgress(string questId)
    {
        if (IsInvalidId(questId))
        {
            Debug.LogError("[QuestProgress] questId가 비어 있습니다.");
            QuestId = string.Empty;
            State = QuestState.NotStarted;
            return;
        }

        QuestId = questId;
        State = QuestState.NotStarted;
    }

    private static bool IsInvalidId(string id)
    {
        return string.IsNullOrWhiteSpace(id);
    }


    // 퀘스트의 생명주기 상태만 갱신한다.
    public void SetState(QuestState state) =>  State = state;

    // 아직 기록되지 않은 목표 진행도는 0으로 반환한다.
    public int GetCounter(string objectiveId)
    {
        if (IsInvalidId(objectiveId))
        {
            Debug.LogWarning("[QuestProgress] objectiveId가 비어 있어 진행도를 조회할 수 없습니다.");
            return 0;
        }

        return objectiveCounters.TryGetValue(objectiveId, out int value) ? value : 0;
    }

    // 비정상 저장 데이터를 막기 위해 목표 진행도는 0 이상으로 제한한다.
    public void SetCounter(string objectiveId, int value)
    {
        if (IsInvalidId(objectiveId))
        {
            Debug.LogWarning("[QuestProgress] objectiveId가 비어 있어 진행도를 저장할 수 없습니다.");
            return;
        }

        objectiveCounters[objectiveId] = Mathf.Max(0, value);
    }

    // 누적형 목표의 진행도를 증가시킨다.
    public void AddCounter(string objectiveId, int amount) => SetCounter(objectiveId, GetCounter(objectiveId) + amount);

    // 퀘스트 상태를 직접 바꾸지 않고 특정 목표만 완료 처리한다.
    public void CompleteObjective(string objectiveId)
    {
        if (IsInvalidId(objectiveId))
        {
            Debug.LogWarning("[QuestProgress] objectiveId가 비어 있어 목표를 완료 처리할 수 없습니다.");
            return;
        }

        completeObjectiveIds.Add(objectiveId);
    }

    // 특정 목표가 이미 완료됐는지 확인한다.
    public bool IsObjectiveCompleted(string objectiveId)
    {
        if (IsInvalidId(objectiveId))
        {
            return false;
        }

        return completeObjectiveIds.Contains(objectiveId);
    }

}
