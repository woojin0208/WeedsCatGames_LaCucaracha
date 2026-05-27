using UnityEngine;

// NPC 이벤트를 통해 씬 전환을 시작하는 입장 지점이다.
public class NPCEnterance : Enterance
{
    public void WarpScene()
    {
        EnterArea(EnteranceType.NPC);
    }
}