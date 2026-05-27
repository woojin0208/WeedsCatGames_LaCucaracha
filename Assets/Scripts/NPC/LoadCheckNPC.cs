using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// NPC 조건에 따라 이벤트 실행, 자동 상호작용, 상태 변경을 처리한다.
public class LoadCheckNPC : MonoBehaviour
{
    [SerializeField] private NPCDialogue targetNPC;
    [SerializeField] private UnityEvent[] events;
    [SerializeField] private NPCState[] targetStates;

    private IEnumerator Start()
    {
        yield return null;

        yield return WaitForSceneTransition();

        InvokeStartEvents();
    }

    private IEnumerator WaitForSceneTransition()
    {
        UIManager uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogWarning("[LoadCheckNPC] UIManager 가 null 입니다.", this);
            yield break;
        }

        while (uiManager.IsSceneTransitioning)
        {
            yield return null;
        }
    }

    private void InvokeStartEvents()
    {
        if (events == null || events.Length == 0) return;

        for (int i = 0; i < events.Length; i++)
        {
            if (events[i] == null) continue;

            events[i].Invoke();
        }
    }

    public void Interactive() => TryInteractiveByState();

    private void TryInteractiveByState()
    {
        if (targetNPC == null)
        {
            Debug.LogWarning("[LoadCheckNPC] targetNPC 가 null 입니다..", this);
            return;
        }

        if (NPCStateManager.Instance == null)
        {
            Debug.LogWarning("[LoadCheckNPC] NPCStateManager 가 씬에 없습니다.", this);
            return;
        }

        if (targetStates == null || targetStates.Length == 0)
        {
            Debug.LogWarning("[LoadCheckNPC] targetStates 가 비어 있습니다.", this);
            return;
        }

        NPCState currentState = NPCStateManager.Instance.GetState(targetNPC.NPCId);

        for (int i = 0; i < targetStates.Length; i++)
        {
            if (targetStates[i] != currentState) continue;

            targetNPC.Interactive();
            return;
        }
    }

    public void SetNPCState(int targetStateNum)
    {
        if (targetNPC == null)
        {
            Debug.LogWarning("[LoadCheckNPC] targetNPC 가 null 입니다.", this);
            return;
        }

        if (targetStates == null || targetStates.Length == 0)
        {
            Debug.LogWarning("[LoadCheckNPC] targetStates 가 비어 있습니다.", this);
            return;
        }

        if (targetStateNum < 0 || targetStateNum >= targetStates.Length)
        {
            Debug.LogWarning($"[LoadCheckNPC] targetStateNum 범위가 잘못되었습니다. index: {targetStateNum}", this);
            return;
        }

        NPCStateManager npcStateManager = NPCStateManager.Instance;
        if (npcStateManager == null)
        {
            Debug.LogWarning("[LoadCheckNPC] npcStateManager 가 null 입니다.", this);
            return;
        }

        npcStateManager.SetState(targetNPC.NPCId, targetStates[targetStateNum]);

    }
}
