using System.Collections;
using UnityEngine;

// 아이템 지급 NPC 동작을 처리한다.
public class ItemGiver : MonoBehaviour
{
    [SerializeField] private QuestData questData;
    [SerializeField] private string objectiveId = "objective_give_item";

    [SerializeField] private NPCDialogue currentNPC;
    [SerializeField] private bool hasNextNode = false;

    public void GiveItem()
    {
        if (!TryGetQuestObjective(out QuestObjectiveData objective)) return;

        WeaponBase targetItem = questData.GetObjective(objectiveId).RewardWeapon;
        if (targetItem == null)
        {
            Debug.LogWarning("[ItemGiver] targetItem 이 null 입니다.", this);
            return;
        }

        PlayerManager pm = PlayerManager.Instance;
        if (pm == null)
        {
            Debug.LogWarning("[ItemGiver] PlayerManager 가 null 입니다.", this);
            return;
        }

        WeaponInstance? addedWeapon = pm.AddWeapon(targetItem.name, targetItem.Durability);
        if (addedWeapon == null)
        {
            Debug.LogWarning("[ItemGiver] 인벤토리에 빈 슬롯이 없습니다.", this);
            return;
        }

        MarkRewarded();

        if (hasNextNode) StartCoroutine(NextNodeWaitFrame());
    }

    private bool TryGetQuestObjective(out QuestObjectiveData objective)
    {
        objective = null;

        if (questData == null)
        {
            Debug.LogError("[ItemGiver] QuestData 가 null 입니다.", this);
            return false;
        }

        return questData.TryGetObjective(objectiveId, out objective);
    }
    private void MarkRewarded()
    {
        QuestManager questManager = QuestManager.Instance;
        if (questManager == null)
        {
            Debug.LogError("[ItemGiver] QuestManager가 null 입니다.", this);
            return;
        }

        questManager.TryReward(questData.QuestId, objectiveId);

        currentNPC.SetRepeat();
    }
    private IEnumerator NextNodeWaitFrame()
    {
        yield return new WaitForEndOfFrame();

        if (currentNPC == null) yield break;
        currentNPC.Interactive();
    }
}

