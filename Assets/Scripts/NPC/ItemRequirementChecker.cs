using System;
using System.Collections;
using System.Linq;
using UnityEngine;

// 아이템 요구 조건을 검사한다.
public class ItemRequirementChecker : MonoBehaviour
{
    [SerializeField] private QuestData questData;
    [SerializeField] private string objectiveId = "objective_required_item";

    [SerializeField] private NPCDialogue currentNPC;

    public void CheckItem()
    {
        if (currentNPC == null)
        {
            Debug.LogWarning("[ItemRequirementChecker] 연결된 NPC가 없어 아이템 검사 불가", this);
            return;
        }

        if (!TryGetQuestObjective(out QuestObjectiveData objective)) return;

        PlayerManager pm = PlayerManager.Instance;
        if (pm == null)
        {
            Debug.LogWarning("[ItemRequirementChecker] PlayerManager가 없어 아이템 검사 불가", this);
            return;
        }

        WeaponBase currentWeapon = pm.CurrentWeapon;
        if (currentWeapon == null) return;

        bool isMatch = HasRequiredWeapon(currentWeapon, objective.RequiredWeapons);

        if (isMatch)
        {
            CompleteRequirementQuest();
            currentNPC.SetCompleted();
        }
        else
        {
            FailRequirementQuest();
            currentNPC.SetFailed();
        }

        pm.RemoveWeapon(pm.CurrentEquipId, false);
        StartCoroutine(NextNodeWaitFrame());
    }

    private bool TryGetQuestObjective(out QuestObjectiveData objective)
    {
        objective = null;

        if (questData == null)
        {
            Debug.LogError("[itemRequirmentChecker] QuestData 가 null 입니다.", this);
            return false;
        }

        return questData.TryGetObjective(objectiveId, out objective);
    }

    private bool HasRequiredWeapon(WeaponBase currentWeapon, WeaponBase[] requiredWeapons)
    {
        if (currentWeapon == null || requiredWeapons == null || requiredWeapons.Length == 0) return false;

        int index = Array.FindIndex(requiredWeapons, weapon => weapon != null && weapon.name == currentWeapon.name);

        return index != -1;
    }

    private void CompleteRequirementQuest()
    {
        QuestManager questManager = QuestManager.Instance;
        if (questManager == null)
        {
            Debug.LogError("[ItemRequirementChecker] QuestManager가 null 입니다.", this);
            return;
        }

        string questId = questData.QuestId;
        questManager.TryCompleteObjectiveAndState(questId, objectiveId, QuestState.Completed);
    }

    private void FailRequirementQuest()
    {
        QuestManager questManager = QuestManager.Instance;
        if (questManager == null)
        {
            Debug.LogError("[ItemRequirementChecker] QuestManager가 null 입니다.", this);
            return;
        }

        questManager.TrySetState(questData.QuestId, QuestState.Failed);
    }

    private IEnumerator NextNodeWaitFrame()
    {
        yield return new WaitForEndOfFrame();

        if (currentNPC == null) yield break;
        currentNPC.Interactive();
    }
}