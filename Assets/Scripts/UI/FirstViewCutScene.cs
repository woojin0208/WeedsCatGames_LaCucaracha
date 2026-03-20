using UnityEngine;
using UnityEngine.Video;

// 보스 컷신 연출을 제어한다.
public class FirstViewCutScene : MonoBehaviour
{
    private UIManager uiManager;
    [SerializeField] VideoPlayer vPlayer;
    [SerializeField] private AnchoredBossController bossController;
    [SerializeField] private GameObject player;

    [SerializeField] BossUI bossUI;

    private void Awake()
    {
        uiManager = UIManager.Instance;
    }

    private void OnEnable()
    {
        if (vPlayer != null)
            vPlayer.loopPointReached += OnVideoEnd;

        SetInputEventSubscription(true);
    }

    private void OnDisable()
    {
        if (vPlayer != null)
            vPlayer.loopPointReached -= OnVideoEnd;

        SetInputEventSubscription(false);
    }

    private void Start()
    {
        if (uiManager == null) uiManager = UIManager.Instance;

        if (uiManager.canViewVideo)
        {
            InputStateManager.Instance?.ChangeState(InputStateType.Gameplay);
            vPlayer.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            InputStateManager.Instance?.ChangeState(InputStateType.CutScene);
            uiManager.gameObject.SetActive(false);
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (uiManager == null) uiManager = UIManager.Instance;

        InputStateManager.Instance?.ChangeState(InputStateType.Gameplay);
        uiManager.gameObject.SetActive(true);
        uiManager.canViewVideo = true;
        player.transform.position = new Vector2(-6, -3);
        bossController.StartBattle();
        bossUI.StartBossBattle();

        vPlayer.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void HandleCutsceneSkipRequested()
    {
        if (!gameObject.activeInHierarchy) return;
        OnVideoEnd(vPlayer);
    }

    private void SetInputEventSubscription(bool isSubscribe)
    {
        InputStateManager manager = InputStateManager.Instance;
        if (manager == null) return;

        manager.CutsceneSkipRequested -= HandleCutsceneSkipRequested;
        if (isSubscribe)
        {
            manager.CutsceneSkipRequested += HandleCutsceneSkipRequested;
        }
    }
}
