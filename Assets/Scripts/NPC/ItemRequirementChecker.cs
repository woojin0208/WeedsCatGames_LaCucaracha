using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            Debug.Log("엥 이러면 안되는 거 아니에오..>?");
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
