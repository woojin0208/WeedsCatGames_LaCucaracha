using System;
using System.Collections;
using UnityEngine;

// 기부 상자 상호작용을 처리한다.
public class DonationBox : MonoBehaviour
{
    [SerializeField] private QuestData questData;
    [SerializeField] private string objectiveId = "objective_donation_score";

    [SerializeField] private WeaponBase[] requirments;
    [SerializeField] private int[] itemScores;

    [SerializeField] private NPCDialogue currentNPC;

    [SerializeField] private DialogueNodeData donationInprogressDialogue;

    [SerializeField] private Animator boxAnimator;

    [SerializeField] private VFXPlayer vfxPlayer;

    public void DonationItem()
    {
        boxAnimator?.SetTrigger("Interaction");

        if (!TryGetQuestObjective(out QuestObjectiveData objective)) return;

        PlayerManager pm = PlayerManager.Instance;
        WeaponBase currentWeapon = pm.CurrentWeapon;
        if (currentWeapon == null) return;
        if (requirments == null || itemScores == null) return;
        if (currentNPC == null)
        {
            Debug.LogWarning("[DonationBox] 연결된 NPC가 없어 기부 처리를 진행할 수 없습니다.", this);
            return;
        }

        int index = Array.FindIndex(requirments, w => w != null && w.name == currentWeapon.name);
        if (index == -1) return;
        if (index >= itemScores.Length)
        {
            Debug.LogWarning("[DonationBox] 요구 아이템과 점수 배열 길이가 맞지 않습니다.", this);
            return;
        }

        int itemScore = itemScores[index];
        if (!TryAddDonationScore(itemScore, out int currentScore)) return;

        vfxPlayer?.StartVFX(0);
        pm.RemoveWeapon(pm.CurrentEquipId, false);

        int targetScore = objective.TargetValue;

        SetDonationProgressDialogue(itemScore, currentScore, targetScore);

        if (currentScore >= targetScore)
        {
            CompleteDonationQuest();
        }

        StartCoroutine(NextNodeWaitFrame());
    }

    private bool TryGetQuestObjective(out QuestObjectiveData objective)
    {
        objective = null;

        if (questData == null)
        {
            Debug.LogError("[Donation Box] QuestData 가 null 입니다.");
            return false;
        }

        return questData.TryGetObjective(objectiveId, out objective);
    }

    private void SetDonationProgressDialogue(int itemScore, int currentScore, int targetScore)
    {
        if (donationInprogressDialogue == null || donationInprogressDialogue.texts == null || donationInprogressDialogue.texts.Length == 0)
        {
            Debug.LogWarning("[DonationBox] 기부 진행 대화 데이터가 비어 있습니다.", this);
            return;
        }

        var progressNode = donationInprogressDialogue.CloneRuntime();
        progressNode.texts[0] = $"{itemScore} 만큼의 가치군. (누적 {currentScore}/{targetScore})";
        currentNPC.SetEntry(progressNode);
    }

    private bool TryAddDonationScore(int score, out int currentScore)
    {
        currentScore = 0;

        QuestManager questManager = QuestManager.Instance;
        if (questManager == null)
        {
            Debug.LogError("[DonationBox] QuestManager가 없어 기부 진행도를 갱신할 수 없습니다.", this);
            return false;
        }

        return questManager.TryAddCounter(questData.QuestId, objectiveId, score, out currentScore);
    }

    private void CompleteDonationQuest()
    {
        QuestManager questManager = QuestManager.Instance;

        if (questManager != null)
        {
            questManager.TryCompleteObjectiveAndState(questData.QuestId, objectiveId, QuestState.Completed);
        }

        vfxPlayer?.StartVFX(1);
        currentNPC.SetCompleted();
    }

    private IEnumerator NextNodeWaitFrame()
    {
        yield return new WaitForEndOfFrame();

        if (currentNPC == null) yield break;
        currentNPC.Interactive();
    }
}
