using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IDialogueNode
{
    public IReadOnlyList<string> Texts { get; }
    public IReadOnlyList<IDialogueOption> Options { get; }
}

public interface IDialogueOption
{ 
    public string Label { get; }
    public IDialogueNode Next { get; }
}

[CreateAssetMenu(fileName = "DialogueNodeData", menuName = "Game/DialogueNodeData")]
public class DialogueNodeData : ScriptableObject, IDialogueNode
{
    public string entityName;

    [TextArea]
    public string[] texts;
    public DialogueOption[] options;

    public IReadOnlyList<string> Texts => texts;
    public IReadOnlyList<IDialogueOption> Options => options;

    public DialogueNodeData CloneRuntime()
    {
        var clone = Instantiate(this);
        clone.texts = (string[])texts.Clone();       // 배열 깊은 복사
        clone.options = (DialogueOption[])options.Clone(); // 참조 배열 복사
        return clone;
    }
}

[System.Serializable]
public class DialogueOption : IDialogueOption
{
    public string label;
    public DialogueNodeData nextNode;
    public UnityEvent onSelected; // ★ 추가: 옵션마다 이벤트
    public string Label => label;
    public IDialogueNode Next => nextNode;
    public void Invoke() => onSelected?.Invoke();
}
