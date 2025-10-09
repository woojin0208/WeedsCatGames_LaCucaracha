using UnityEngine;

public enum QuestState { None, Available, Accepted, InProgress, TurnInReady, Completed, Failed }
public enum QuestId { SampleA, SampleB }

[CreateAssetMenu(menuName = "Game/Quest/Definition")]
public class QuestDefinition : ScriptableObject
{
    public QuestId Id;
    public NPCId Giver;
    public NPCId TurnInNpc;
    public bool Repeatable;

    [Header("Dialogue Nodes")]
    public DialogueNodeData ReQuest;     // 수주
    public DialogueNodeData InProgress;  // 진행 중
    public DialogueNodeData Complete;    // 완료 보고
    public DialogueNodeData Repeat;      // 완료 이후
    public DialogueNodeData Fail;        // 실패
}
