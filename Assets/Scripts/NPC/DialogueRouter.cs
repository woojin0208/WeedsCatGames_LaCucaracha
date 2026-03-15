using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// NPC 대화 분기를 연결한다.
public class DialogueRouter : MonoBehaviour
{
    [SerializeField] private DialogueNodeData nodeData;
    [SerializeField] private UnityEvent nodeEvent;
    private NPCDialogue targetNPC;
    public void StartDialogueWithNode(NPCDialogue targetNPC)
    {
        Debug.Log("여긴가??????????????");
        this.targetNPC = targetNPC;

        targetNPC.OnDialogueSignal += HandleStartDialogue;
    }

    private void HandleStartDialogue()
    {
        Debug.Log("여긴가??????????????");
        targetNPC.StartDialogueWithNode(nodeData);

        StartCoroutine(nameof(WaitFrameEvent));
    }

    public void WaitForFrame() => StartCoroutine(nameof(WaitFrameEvent));
    private IEnumerator WaitFrameEvent()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("흠 이게 되야 하는데");
        nodeEvent?.Invoke();
    }
    private void OnDisable()
    {
        if (targetNPC != null) targetNPC.OnDialogueSignal -= HandleStartDialogue;
    }
}