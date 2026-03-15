using UnityEngine;

// 경비 NPC 상태에 따라 통행 여부가 결정되는 입장 지점이다.
public class GuardedEnterance : InteractableEnterance
{
    [SerializeField] private NPCDialogue guardNPC;
    [SerializeField] private DialogueNodeData guardNode;

    protected override void EnterArea(string nextArea, EnteranceType enterance)
    {
        if (guardNPC == null)
        {
            base.EnterArea(nextArea, EnteranceType.Guard);
        }
        else if (!guardNPC.gameObject.activeInHierarchy || !guardNPC.gameObject.activeSelf)
        {
            base.EnterArea(nextArea, EnteranceType.Guard);
        }
        else
        {
            if ((int)NPCStateManager.Instance.GetState(guardNPC.NPCId) >= (int)NPCState.Completed) base.EnterArea(nextArea, EnteranceType.Guard);
            else
            {
                if (guardNode != null)
                    guardNPC.StartDialogueWithNode(guardNode);
                else
                {
                    guardNPC.Interactive();
                }
            }
        }
    }
}