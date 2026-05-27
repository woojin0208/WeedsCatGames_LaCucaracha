using System.Collections.Generic;
using UnityEngine;

// 대화 노드가 제공해야 하는 읽기 전용 데이터 규약이다.
public interface IDialogueNode
{
    public IReadOnlyList<string> Texts { get; }
    public IReadOnlyList<IDialogueOption> Options { get; }
}

// 대화 선택지가 제공해야 하는 읽기 전용 데이터 규약이다.
public interface IDialogueOption
{
    public string Label { get; }
    public IDialogueNode Next { get; }
}

// 대화 본문과 선택지 연결 정보를 저장하는 ScriptableObject다.
[CreateAssetMenu(fileName = "DialogueNodeData_", menuName = "Game/Dialogue/Dialogue Node Data")]
public class DialogueNodeData : ScriptableObject, IDialogueNode
{
    [field: SerializeField] public string entityName { get; private set; }

    [field: SerializeField, TextArea] public string[] texts { get; private set; }
    [field: SerializeField] public DialogueOption[] options { get; private set; }

    public IReadOnlyList<string> Texts => texts;
    public IReadOnlyList<IDialogueOption> Options => options;

    public DialogueNodeData CloneRuntime()
    {
        DialogueNodeData clone = Instantiate(this);

        clone.texts = Texts != null ? (string[])texts.Clone() : null;
        clone.options = Options != null ? (DialogueOption[])options.Clone() : null;

        return clone;
    }
}

// 대화 선택지 데이터를 정의한다.
[System.Serializable]
public class DialogueOption : IDialogueOption
{
    [field: SerializeField] public string Label { get; private set; }
    [field: SerializeField] public DialogueNodeData nextNode { get; private set; }

    public IDialogueNode Next => nextNode;
}
