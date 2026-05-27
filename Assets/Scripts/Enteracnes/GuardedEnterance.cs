using UnityEngine;

// 경비 NPC 상태에 따라 통행 여부가 결정되는 입장 지점이다.
public class GuardedEnterance : InteractableEnterance
{
    [SerializeField] private NPCDialogue guardNPC;
    [SerializeField] private DialogueNodeData guardNode;

    protected override bool EnterArea(EnteranceType enterance)
    {
        if (CanPassGuard())
        {
            return base.EnterArea(EnteranceType.Guard);
        }

        StartGuardDialogue();
        return false;
    }

    private bool CanPassGuard()
    {
        if (guardNPC == null) return true;
        if (!guardNPC.gameObject.activeInHierarchy) return true;

        NPCStateManager stateManager = NPCStateManager.Instance;
        if (stateManager == null)
        {
            Debug.LogWarning("[GuardEnterance] NPCStateManager 가 null 입니다.", this);
            return false;
        }

        NPCState guardState = stateManager.GetState(guardNPC.NPCId);
        return guardState == NPCState.Completed;
    }

    private void StartGuardDialogue()
    {
        if (guardNPC == null)
        {
            Debug.LogWarning("[GuardedEnterance] guardNPC 가 null 입니다.", this);
            return;
        }

        if (guardNode != null)
        {
            guardNPC.StartDialogueWithNode(guardNode);
            return;
        }

        guardNPC.Interactive();
    }
}