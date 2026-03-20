using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// 저장 상태에 따라 NPC 노출을 제어한다.
public class LoadCheckNPC : MonoBehaviour
{
    [SerializeField] private NPCDialogue targetNPC;
    [SerializeField] private UnityEvent[] events;
    [SerializeField] private NPCState[] targetStates;
    private void Start()
    {
        if (events.Length > 0)
        {
            for (int i = 0; i < events.Length; i++)
            {
                events[i]?.Invoke();
            }
        }
    }

    public void Interactive() => Invoke(nameof(AutoInteractive), 0.55f);

    private void AutoInteractive()
    {
        bool isTarget = targetStates.Any(s => NPCStateManager.Instance.GetState(targetNPC.NPCId) == s);

        if (isTarget) targetNPC.Interactive();
    }

    public void SetNPCState(int targetStateNum)
    {
        NPCStateManager.Instance.SetState(targetNPC.NPCId, targetStates[targetStateNum]);
    }
}
