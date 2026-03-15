using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 대화 노드가 제공해야 하는 읽기 전용 데이터를 정의한다.
public interface IDialogueNode
{
    public IReadOnlyList<string> Texts { get; }
    public IReadOnlyList<IDialogueOption> Options { get; }
}

// 대화 선택지가 제공해야 하는 읽기 전용 데이터를 정의한다.
public interface IDialogueOption
{ 
    public string Label { get; }
    public IDialogueNode Next { get; }
}

// 대사 본문과 선택지 정보를 저장하는 대화 노드 에셋이다.
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
        clone.texts = (string[])texts.Clone(); // 런타임에서 원본 에셋을 직접 수정하지 않도록 배열을 복사한다.
        clone.options = (DialogueOption[])options.Clone();
        return clone;
    }
}

[System.Serializable]

// 대화 선택지 데이터를 정의한다.
public class DialogueOption : IDialogueOption
{
    public string label;
    public DialogueNodeData nextNode;
    public UnityEvent onSelected;
    public string Label => label;
    public IDialogueNode Next => nextNode;
    public void Invoke() => onSelected?.Invoke();
}
