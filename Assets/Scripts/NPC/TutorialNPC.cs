using TMPro;
using UnityEngine;

// 튜토리얼 안내 문구를 출력하는 특수 NPC다.
// 일반 NPC 대화 노드 흐름을 사용하지 않고, 튜토리얼 진행에 따라 UI 텍스트를 직접 갱신한다.
public class TutorialNPC : NPCDialogue
{
    [SerializeField] private EnemyBase enemyBase;

    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private TextMeshProUGUI textIndex;

    [SerializeField] private EnvironmentRenderer speakerRenderer;

    [TextArea][SerializeField] private string[] tutorialTexts;

    [TextArea][SerializeField] private string guardText;

    private int currentTextIndex = 0;

    // NPCDialogueData 기반 대화 검증을 통과시킨다.
    protected override bool ValidateDialogueData()
    {
        return true;
    }
    protected override void Awake()
    {
        base.Awake();

        if (enemyBase != null)
        {
            enemyBase.OnDamagedAction -= ViewDialogue;
            enemyBase.OnDamagedAction += ViewDialogue;
        }
        else
        {
            Debug.LogWarning("[TutorialNPC] EnemyBase 가 null 입니다.");
        }

        ViewDialogue();
    }

    private void ViewDialogue()
    {
        OnSpeak();

        if (tutorialTexts == null ||  tutorialTexts.Length == 0)
        {
            Debug.LogWarning("[tutorialNPC 가 비어 있습니다.", this);
            return;
        }

        if (currentTextIndex >= tutorialTexts.Length - 1) NPCStateManager.Instance.SetState(NPCId, NPCState.Completed);
        
        if (currentTextIndex >= tutorialTexts.Length) currentTextIndex = 0;

        if (tutorialText != null) tutorialText.text = tutorialTexts[currentTextIndex];

        if (textIndex != null) textIndex.text = $"{currentTextIndex + 1} / {tutorialTexts.Length}";

        currentTextIndex++;
    }

    public override void Interactive(PlayerBase _ = null)
    {
        OnSpeak();

        if (tutorialText != null) tutorialText.text = guardText;
    }

    private void OnSpeak() => speakerRenderer?.Interactive();
}