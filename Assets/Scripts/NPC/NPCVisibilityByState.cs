using System.Linq;
using UnityEngine;

// NPC 상태에 따라 표시 여부를 전환한다.
public class NPCVisibilityByState : MonoBehaviour
{
    [SerializeField] private NPCDialogue targetNPC;
    [SerializeField] private NPCState[] visibilityStates;

    private void Start()
    {
        CheckVisibility(NPCStateManager.Instance.GetState(targetNPC.NPCId));
    }

    private void CheckVisibility(NPCState state)
    {
        bool isVisible = visibilityStates.Any(s => NPCStateManager.Instance.GetState(targetNPC.NPCId) == s);

        if (isVisible) return;

        targetNPC.gameObject.SetActive(false);
    }
}