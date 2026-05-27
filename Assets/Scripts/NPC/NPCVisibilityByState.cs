using System.Linq;
using UnityEngine;

// NPCState에 따라 대상 NPC 오브젝트의 표시 여부를 결정한다.
public class NPCVisibilityByState : MonoBehaviour
{
    [SerializeField] private NPCDialogue targetNPC;
    [SerializeField] private NPCState[] visibilityStates;

    private void Start()
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (targetNPC == null)
        {
            Debug.LogWarning("[NPCVisibilityByState] targetNPC가 설정되지 않았습니다.", this);
            return;
        }

        if (visibilityStates == null || visibilityStates.Length == 0)
        {
            targetNPC.gameObject.SetActive(false);
            return;
        }
        NPCStateManager npcStateManager = NPCStateManager.Instance;
        if (npcStateManager == null)
        {
            Debug.LogWarning("[NPCVisibilityByState] NPCStateManager 가 null 입니다.");
            return;
        }

        NPCState currentState = npcStateManager.GetState(targetNPC.NPCId);

        bool isVisible = visibilityStates.Any(state => state == currentState);

        targetNPC.gameObject.SetActive(isVisible);
    }
}