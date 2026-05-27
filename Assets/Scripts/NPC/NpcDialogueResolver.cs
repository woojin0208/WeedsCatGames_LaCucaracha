using UnityEngine;

// NPC의 현재 상태와 일회성 entry node를 기준으로 시작 대화를 결정한다.
public class NpcDialogueResolver
{
    public bool TryResolve(
        NPCDialogueData dialogueData,
        NPCId npcId,
        DialogueNodeData entryNode,
        out DialogueNodeData startNode)
    {
        startNode = null;

        // entry는 현재 상태보다 우선되는 일회성 노드.
        if (entryNode != null)
        {
            startNode = entryNode;
            return true;
        }

        if (dialogueData == null)
        {
            Debug.LogError($"[NPCDialogueResolver] NPCDialougeData 가 없습니다. npcId: {npcId}");
            return false;
        }

        NPCStateManager npcStateManager = NPCStateManager.Instance;
        if (npcStateManager == null)
        {
            Debug.LogError($"[NPCDialogueResolver] NPCStateManager 가 null 입니다.");
            return false;
        }

        NPCState state = npcStateManager.GetState(npcId);
        if (dialogueData.TryGetNode(state, out startNode))
        {
            return true;
        }

        Debug.LogWarning($"[NPCDialogueResolver] 시작 대화를 찾을 수 없습니다. npcId: {npcId}, state: {state}");
        return false;
    }
}
