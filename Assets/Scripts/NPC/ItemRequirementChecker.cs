using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 아이템 요구 조건을 검사한다.
public class ItemRequirementChecker : MonoBehaviour
{
    [SerializeField] private NPCDialogue currentNPC;

    [SerializeField] private WeaponBase[] requirments;

    public void CheckItem()
    {
        Debug.Log("TryCheckItem");
        PlayerManager pm = PlayerManager.Instance;
        WeaponBase currentWeapon = pm.CurrentWeapon;
        if (currentWeapon == null) return;
        Debug.Log(currentWeapon);

        bool isMatch = requirments.Any(w => w.name == currentWeapon.name);
        if (isMatch)
        {
            currentNPC.SetCompleted();
            pm.RemoveWeapon(pm.CurrentEquipId, false);
        }
        else
        {
            currentNPC.SetFailed();
            pm.RemoveWeapon(pm.CurrentEquipId, false);
        }

        StartCoroutine(NextNodeWaitFrame());
    }

    private IEnumerator NextNodeWaitFrame()
    {
        yield return new WaitForEndOfFrame();

        currentNPC.Interactive();
    }
}