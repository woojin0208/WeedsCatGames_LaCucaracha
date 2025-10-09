using UnityEngine;

[CreateAssetMenu(menuName = "Game/NPC Dialogue Preset")]
public class NPCDialoguePreset : ScriptableObject
{
    public NPCId NpcId;
    public DialogueNodeData FirstMeet;
    public DialogueNodeData DefaultRepeat; // 퀘스트 비해당 시 기본 반복 대사
}