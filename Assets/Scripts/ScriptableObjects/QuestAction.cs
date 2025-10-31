// QuestAction.cs
using Unity.VisualScripting;
using UnityEngine;

public struct QuestContext
{
    public string QuestId;
    public QuestDefinition Definition;
    public QuestProgress Progress;
    public QuestJournal Journal;

}

public abstract class QuestAction : ScriptableObject
{
    public abstract void Execute(QuestContext ctx);
}

[CreateAssetMenu(menuName = "Game/QuestActions/GoTo")]
public class GoToAction : QuestAction
{
    [SerializeField] private string areaId;

    public override void Execute(QuestContext ctx)
    {
        ctx.Journal.AddProgress(ObjectiveType.GoTo, areaId, 1);
    }
}
