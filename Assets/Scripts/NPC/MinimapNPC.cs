using UnityEngine;
using UnityEngine.Events;

public class MinimapNPC : NPCDialogue
{
    public void OpenMinimap()
    {
        UIManager.Instance.OpenMinimap();
        
    }
}
