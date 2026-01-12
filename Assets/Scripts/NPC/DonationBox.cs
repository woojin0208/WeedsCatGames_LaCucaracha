using System;
using System.Collections;
using UnityEngine;

public class DonationBox : MonoBehaviour
{
    [SerializeField] private int targetScore = 20;

    [SerializeField] private WeaponBase[] requirments;
    [SerializeField] private int[] itemScores;

    [SerializeField] private NPCDialogue currentNPC;

    [SerializeField] private DialogueNodeData donationInprogressDialogue;

    [SerializeField] private Animator boxAnimator;

    [SerializeField] private VFXPlayer vfxPlayer;

    public void DonationItem()
    {
        boxAnimator.SetTrigger("Interaction");
        //Debug.LogAssertion("들어오긴 함");
        PlayerManager pm = PlayerManager.Instance;
        WeaponBase currentWeapon = pm.CurrentWeapon;
        if (currentWeapon == null) return;

        int index = Array.FindIndex(requirments, w => w.name == currentWeapon.name);
        if (index != -1)
        {
            vfxPlayer.StartVFX(0);
            GameManager.Instance.donationScore += itemScores[index];
            pm.RemoveWeapon(pm.CurrentEquipId, false);

            var progressNode = donationInprogressDialogue.CloneRuntime();
            progressNode.texts[0] = $"{itemScores[index]} 만큼의 가치군. (누적 {GameManager.Instance.donationScore}/{targetScore})";
            currentNPC.SetEntry(progressNode);
            StartCoroutine(NextNodeWaitFrame());
        }

        if (GameManager.Instance.donationScore >= targetScore)
        {
            vfxPlayer.StartVFX(1);
            currentNPC.SetCompleted();

            StartCoroutine(NextNodeWaitFrame());
        }
    }

    private IEnumerator NextNodeWaitFrame()
    {
        yield return new WaitForEndOfFrame();

        currentNPC.Interactive();
    }
}
