using System.Collections;
using UnityEngine;

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
