using UnityEngine;
using UnityEngine.Video;

// 보스 컷신 재생, 스킵, 전투 시작 흐름을 제어한다.
public class BossIntroCutsceneController : MonoBehaviour
{
    private UIManager uiManager;

    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private BGMPlayer bgmPlayer;
    
    [SerializeField] private AnchoredBossController bossController;
    [SerializeField] private BossUI bossUI;

    [SerializeField] private GameObject player;
    [SerializeField] private Transform playerStartPoint;

    private bool isFinished;

    private void Awake()
    {
        uiManager = UIManager.Instance;

        if (bgmPlayer == null)
            bgmPlayer = FindObjectOfType<BGMPlayer>();
    }

    private void OnEnable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoEnd;

        SetInputEventSubscription(true);
    }

    private void OnDisable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoEnd;

        SetInputEventSubscription(false);
    }

    private void Start()
    {
        uiManager = UIManager.Instance;

        if (uiManager == null)
        {
            Debug.LogWarning("[BossIntroCutsceneController] UIManager 가 null 입니다.", this);
            return;
        }

        if (uiManager.canViewVideo)
        {
            SkipAlreadyViewedCutscene();
            return;
        }

        StartCutscene();
    }

    private void StartCutscene()
    {
        InputStateManager.Instance?.ChangeState(InputStateType.CutScene);

        if (uiManager != null)
        {
            uiManager.gameObject.SetActive(false);
        }

        if (videoPlayer != null)
        {
            videoPlayer.gameObject.SetActive(true);
        }
    }

    private void SkipAlreadyViewedCutscene()
    {
        InputStateManager.Instance?.ChangeState(InputStateType.Gameplay);

        if (videoPlayer != null) videoPlayer.gameObject.SetActive(false);
    }
    private void OnVideoEnd(VideoPlayer vp)
    {
        FinishCutscene(false);
    }

    private void MovePlayerToStartPoint()
    {
        if (player == null || playerStartPoint == null) return;

        player.transform.position = playerStartPoint.position;
    }

    private void StartBossBattle()
    {
        bossController?.StartBattle();
        bossUI?.StartBossBattle();
    }

    private void HandleCutsceneSkipRequested()
    {
        if (!gameObject.activeInHierarchy) return;
        FinishCutscene(true);
    }

    private void FinishCutscene(bool shouldChangeBGM)
    {
        if (isFinished) return;

        isFinished = true;

        if (shouldChangeBGM)
        {
            bgmPlayer?.StartBGM(0);
        }

        InputStateManager.Instance?.ChangeState(InputStateType.Gameplay);

        if (uiManager == null) uiManager = UIManager.Instance;

        if (uiManager != null)
        {
            uiManager.gameObject.SetActive(true);
            uiManager.canViewVideo = true;
        }

        MovePlayerToStartPoint();
        StartBossBattle();

        if (videoPlayer != null) videoPlayer.gameObject.SetActive(false);

        gameObject.SetActive(false);
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
