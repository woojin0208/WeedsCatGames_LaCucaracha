using System.Collections;
using UnityEngine;

// 아이템 지급 NPC 동작을 처리한다.
public class ItemGiver : MonoBehaviour
{
    [SerializeField] private WeaponBase targetItem;
    [SerializeField] private NPCDialogue currentNPC;
    [SerializeField] private bool hasNextNode = false;
    public void GiveItem()
    {
        PlayerManager pm = PlayerManager.Instance;

        pm.AddWeapon(targetItem.name, targetItem.Durability);

        if (hasNextNode)
            StartCoroutine(NextNodeWaitFrame());
    }
    private IEnumerator NextNodeWaitFrame()
    {
        yield return new WaitForEndOfFrame();

        currentNPC.Interactive();
    }
}