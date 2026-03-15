using UnityEngine;

// NPC 대화 프리셋 데이터를 저장하는 에셋이다.
[CreateAssetMenu(menuName = "Game/NPC Dialogue Preset")]
public class NPCDialoguePreset : ScriptableObject
{
    public NPCId NpcId;
    public DialogueNodeData FirstMeet;
    public DialogueNodeData DefaultRepeat;
}