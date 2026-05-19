using System.Collections.Generic;
using UnityEngine;

// 퀘스트 런타임 진행 상태에 접근하는 중심 진입점이다.
// 다른 시스템은 퀘스트 진행 상태를 직접 보유하지 않고 이 매니저에 변경을 요청한다.
public class QuestManager : MonoBehaviour
{
    private static QuestManager instance;
    public static QuestManager Instance => instance;

    // 안정적인 questId를 기준으로 런타임 퀘스트 진행 상태를 관리한다.
    private readonly Dictionary<string, QuestProgress> progressByQuestId = new();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private static bool IsInvalidId(string id) => string.IsNullOrWhiteSpace(id);

    // 기존 진행 상태를 반환하고, 처음 참조되는 퀘스트라면 기본 진행 상태를 생성한다.
    public QuestProgress GetOrCreateProgress(string questId)
    {
        if (IsInvalidId(questId))
        {
            Debug.LogError("[QuestManager] questId가 비어 있어 진행 상태를 생성할 수 없습니다.");
            return null;
        }

        if (!progressByQuestId.TryGetValue(questId, out QuestProgress progress))
        {
            progress = new QuestProgress(questId);
            progressByQuestId.Add(questId, progress);
        }

        return progress;
    }

    // 퀘스트의 현재 생명주기 상태를 조회한다.
    public QuestState GetState(string questId)
    {
        QuestProgress progress = GetOrCreateProgress(questId);
        return progress != null ? progress.State : QuestState.NotStarted;
    }

    public bool TrySetState(string questId, QuestState state)
    {
        QuestProgress progress = GetOrCreateProgress(questId);
        if (progress == null) return false;

        progress.SetState(state);
        return true;
    }

    // 조건 판정에서는 보상 수령 상태도 완료 상태로 취급한다.
    public bool IsCompleted(string questId)
    {
        QuestState state = GetState(questId);
        return state == QuestState.Completed || state == QuestState.Rewarded;
    }

    // 누적형 목표의 현재 진행도를 조회한다.
    public int GetCounter(string questId, string objectiveId)
    {
        QuestProgress progress = GetOrCreateProgress(questId);
        return progress != null ? progress.GetCounter(objectiveId) : 0;
    }

    public bool TryAddCounter(string questId, string objectiveId, int amount, out int currentValue)
    {
        currentValue = 0;

        if (IsInvalidId(objectiveId))
        {
            Debug.LogError("[QuestManager] objectiveId가 비어 있습니다.");
            return false;
        }

        QuestProgress progress = GetOrCreateProgress(questId);
        if (progress == null) return false;

        progress.AddCounter(objectiveId, amount);
        currentValue = progress.GetCounter(objectiveId);
        return true;
    }

    // 특정 목표 하나를 완료 처리한다.
    public bool TryCompleteObjective(string questId, string objectiveId)
    {
        if (IsInvalidId(objectiveId))
        {
            Debug.LogError("[QuestManager] objectiveId 가 비어 있습니다.");
            return false;
        }

        QuestProgress progress = GetOrCreateProgress(questId) ;
        if (progress == null) return false;

        progress.CompleteObjective(objectiveId);
        return true;
    }

    public bool TryCompleteObjectiveAndState(string questId, string objectiveId, QuestState state)
    {
        if (!TryCompleteObjective(questId, objectiveId)) return false;
        return TrySetState(questId, state);
    }
    public bool TryReward(string questId, string objectiveId)
    {
        if (!string.IsNullOrWhiteSpace(objectiveId))
        {
            if (!TryCompleteObjective(questId, objectiveId)) return false;
        }

        return TrySetState(questId, QuestState.Rewarded);
    }
}
