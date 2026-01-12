using TMPro;
using UnityEngine;

public class TutorialNPC : NPCDialogue
{
    [SerializeField] private EnemyBase enemyBase;

    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private TextMeshProUGUI textIndex;

    [SerializeField] private EnvironmentRenderer speakerRenderer;

    [TextArea][SerializeField] private string[] tutorialTexts;

    [TextArea][SerializeField] private string guardText;

    private int currentTextIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        enemyBase.OnDamagedAction += ViewDialogue;

        ViewDialogue();
    }

    private void ViewDialogue()
    {
        OnSpeak();

        if (currentTextIndex >= tutorialTexts.Length - 1) NPCStateManager.Instance.SetState(NPCId, NPCState.Completed);
        if (currentTextIndex >= tutorialTexts.Length) currentTextIndex = 0;


        tutorialText.text = tutorialTexts[currentTextIndex];
        textIndex.text = $"{currentTextIndex + 1} / {tutorialTexts.Length}";

        currentTextIndex++;
    }

    public override void Interactive(PlayerBase _ = null)
    {
        OnSpeak();

        tutorialText.text = guardText;
        return;
    }

    private void OnSpeak() => speakerRenderer.Interactive();
}