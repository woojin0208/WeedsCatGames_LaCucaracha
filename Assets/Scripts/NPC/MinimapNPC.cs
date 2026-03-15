using UnityEngine;
using UnityEngine.Events;

// 미니맵을 여는 NPC 동작을 정의한다.
public class MinimapNPC : NPCDialogue
{
    public void OpenMinimap()
    {
        UIManager.Instance.OpenMinimap();
    }
}