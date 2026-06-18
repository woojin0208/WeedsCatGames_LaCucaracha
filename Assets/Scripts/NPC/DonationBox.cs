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
        boxAnimator?.SetTrigger(AnimatorParams.Interaction);

        if (currentNPC == null)
        {
            Debug.LogWarning("[DonationBox] 연결된 NPC가 없어 기부 처리를 진행할 수 없습니다.", this);
            return;
        }

        if (!TryGetQuestObjective(out QuestObjectiveData objective)) return;

        PlayerManager playerManager = PlayerManager.Instance;
        if (playerManager == null)
        {
            Debug.LogWarning("[DonationBox] PlayerManager가 null 입니다.", this);
            return;
        }

        WeaponBase currentWeapon = playerManager.CurrentWeapon;
        if (currentWeapon == null)
        {
            Debug.LogWarning("[DonationBox] 현재 장착 중인 무기가 없습니다.", this);
            return;
        }

        if (!TryGetDonationScore(currentWeapon, out int itemScore)) return;
        if (!TryAddDonationScore(itemScore, out int currentScore)) return;

        vfxPlayer?.StartVFX(0);
        playerManager.RemoveWeapon(playerManager.CurrentEquipId, false);

        int targetScore = objective.TargetValue;

        SetDonationProgressDialogue(itemScore, currentScore, targetScore);

        if (currentScore >= targetScore)
        {
            CompleteDonationQuest();
        }

        StartCoroutine(NextNodeWaitFrame());
    }

    private bool TryGetDonationScore(WeaponBase currentWeapon, out int itemScore)
    {
        itemScore = 0;

        if (requirments == null || requirments.Length == 0)
        {
            Debug.LogWarning("[DonationBox] 기부 가능 무기 목록이 비어 있습니다.", this);
            return false;
        }

        if (itemScores == null || itemScores.Length == 0)
        {
            Debug.LogWarning("[DonationBox] 기부 점수 목록이 비어 있습니다.", this);
            return false;
        }

        int index = FindRequirementIndex(currentWeapon);
        if (index == -1)
        {
            Debug.LogWarning($"[DonationBox] 현재 무기가 기부 가능 목록에 없습니다. weapon: {currentWeapon.name}", this);
            return false;
        }

        if (index >= itemScores.Length)
        {
            Debug.LogWarning("[DonationBox] 기부 가능 무기와 점수 배열 길이가 맞지 않습니다.", this);
            return false;
        }

        itemScore = itemScores[index];
        return true;
    }

    private int FindRequirementIndex(WeaponBase currentWeapon)
    {
        if (currentWeapon == null) return -1;

        WeaponDefinition currentDefinition = currentWeapon.WeaponDefinition;

        for (int i = 0; i < requirments.Length; i++)
        {
            WeaponBase requirement = requirments[i];
            if (requirement == null) continue;

            // 변경: 오브젝트 이름보다 WeaponDefinition 참조 비교를 우선한다.
            if (currentDefinition != null && requirement.WeaponDefinition == currentDefinition)
            {
                return i;
            }

            // fallback: 기존 프리팹 이름 기반 비교도 유지한다.
            if (GetNormalizedName(requirement.name) == GetNormalizedName(currentWeapon.name))
            {
                return i;
            }
        }

        return -1;
    }

    private string GetNormalizedName(string weaponName)
    {
        if (string.IsNullOrWhiteSpace(weaponName)) return string.Empty;

        return weaponName.Replace("(Clone)", "").Trim();
    }

    private bool TryGetQuestObjective(out QuestObjectiveData objective)
    {
        objective = null;

        if (questData == null)
        {
            Debug.LogError("[DonationBox] QuestData가 null 입니다.", this);
            return false;
        }

        return questData.TryGetObjective(objectiveId, out objective);
    }

    private void SetDonationProgressDialogue(int itemScore, int currentScore, int targetScore)
    {
        if (donationInprogressDialogue == null ||
            donationInprogressDialogue.texts == null ||
            donationInprogressDialogue.texts.Length == 0)
        {
            Debug.LogWarning("[DonationBox] 기부 진행 대화 데이터가 비어 있습니다.", this);
            return;
        }

        DialogueNodeData progressNode = donationInprogressDialogue.CloneRuntime();
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