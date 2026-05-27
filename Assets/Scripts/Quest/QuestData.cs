using System;
using UnityEngine;

public enum QuestObjectiveType
{
    Counter,
    RequiredWeapon,
    Reward
}

[Serializable]
public class QuestObjectiveData
{
    [field: SerializeField] public string ObjectiveId { get; private set; }
    [field: SerializeField] public QuestObjectiveType ObjectiveType { get; private set; }
    [field: SerializeField] public int TargetValue { get; private set; } = 1;
    [field: SerializeField] public bool IsRetryAllowed { get; private set; } = true;
    [field: SerializeField] public WeaponBase[] RequiredWeapons { get; private set; }
    [field: SerializeField] public WeaponBase RewardWeapon { get; private set; }
}

[CreateAssetMenu(fileName = "QuestData_", menuName = "Game/Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    [field: SerializeField] public string QuestId { get; private set; }
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }
    [field: SerializeField] public QuestObjectiveData[] Objectives { get; private set; }

    public QuestObjectiveData GetObjective(string objectiveId)
    {
        TryGetObjective(objectiveId, out QuestObjectiveData objective);
        return objective;
    }

    public bool TryGetObjective(string objectiveId, out QuestObjectiveData objective)
    {
        objective = null;

        if (string.IsNullOrWhiteSpace(QuestId))
        {
            Debug.LogError("[QuestData] QuestId가 비어 있습니다.", this);
        }
        if (string.IsNullOrWhiteSpace(objectiveId))
        {
            Debug.LogError($"[QuestData] objectiveId 가 비어 있습니다. questId : {QuestId}", this);
            return false;
        }

        if (Objectives == null || Objectives.Length == 0)
        {
            Debug.LogError($"[QuestData] Objectives 가 비어 있습니다. questId : {QuestId}", this);
            return false;
        }

        for (int i = 0; i < Objectives.Length;i++)
        {
            QuestObjectiveData current = Objectives[i];
            if (current == null) continue;

            if (current.ObjectiveId == objectiveId)
            {
                objective = current;
                return true;
            }

        }

        Debug.LogError($"[QuestData] objectiveId 를 찾을 수 없습니다. questId: {QuestId}, objectiveId: {objectiveId}", this);
        return false;
    }
}
