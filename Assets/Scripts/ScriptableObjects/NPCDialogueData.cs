using System;
using UnityEngine;

[Serializable]
public class NPCStateDialogue
{
    [field: SerializeField] public NPCState State { get; private set; }
    [field: SerializeField] public DialogueNodeData Node { get; private set; }
}

[CreateAssetMenu(fileName = "NPCDialogueData_", menuName = "Game/Dialogue/NPC Dialogue Data")]
public class NPCDialogueData : ScriptableObject
{
    [field: SerializeField] public NPCId NpcId { get; private set; }
    [field: SerializeField] public DialogueNodeData FallbackNode { get; private set; }
    [field: SerializeField] public NPCStateDialogue[] StateDialogues { get; private set; }

    // NPCState 와 DialogueNodeData 의 연결을 담당한다.
    public bool TryGetNode(NPCState state, out DialogueNodeData node)
    {
        node = null;

        if (StateDialogues == null || StateDialogues.Length == 0)
        {
            node = FallbackNode;
            return node != null;
        }

        for (int i = 0; i < StateDialogues.Length; i++)
        {
            NPCStateDialogue mapping = StateDialogues[i];
            if (mapping == null) continue;

            if (mapping.State == state)
            {
                node = mapping.Node != null ? mapping.Node : FallbackNode;
                return node != null;
            }
        }

        node = FallbackNode;
        return node != null;
    }

    // NodeEvent에 등록된 노드가 현재 NPCDialogueData에 속하는지 검증한다.
    public bool ContainsNode(DialogueNodeData node)
    {
        if (node == null) return false;
        
        if (FallbackNode == node ) return true;

        if (StateDialogues == null || StateDialogues.Length == 0) return false;

        for (int i = 0; i < StateDialogues.Length; i++)
        {
            NPCStateDialogue mapping = StateDialogues[i];
            if (mapping == null) continue;

            if (mapping.Node == node)
            {
                return true;
            }
        }

        return false;
    }
}
