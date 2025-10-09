using System.Linq;
using UnityEngine;

public class NPCVisibilityByState : MonoBehaviour
{
    [SerializeField] private NPCDialogue targetNPC;
    [SerializeField] private NPCState[] visibilityStates;

    //[SerializeField] private bool checkInRunTime;
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
