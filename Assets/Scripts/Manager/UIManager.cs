using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 UI 패널과 메뉴 상태를 관리한다.
public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get { return instance; }
    }

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject controlSettingPanel;
    [SerializeField] private GameObject audioSettingPanel;
    [SerializeField] private LoadPanel loadPanel;
    [SerializeField] private RectTransform interactiveImage;
    [SerializeField] private GameObject minimap;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private GameObject quitImage;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private WeaponDescriptionUI weaponDescriptionUI;

    public bool IsSceneTransitioning  => loadPanel != null && loadPanel.IsShowing;

    public bool canViewVideo = false;
    private bool isPauseInputSubscribed;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (loadPanel != null)
        {
            loadPanel.HideImmediate();
        }

        inventoryUI?.SetActive(true);
    }

    private void OnEnable()
    {
        SetInputEventSubscription(true);
    }

    private void OnDisable()
    {
        SetInputEventSubscription(false);
    }

    private void Update()
    {
        if (!isPauseInputSubscribed)
        {
            SetInputEventSubscription(true);
        }

        if (Time.timeScale == 0)
        {
            if (interactiveImage != null)
            {
                interactiveImage.gameObject.SetActive(false);
            }
            return;
        }
    }

    private void SetInputEventSubscription(bool isSubscribe)
    {
        InputStateManager manager = InputStateManager.Instance;
        if (manager == null) return;

        if (isSubscribe)
        {
            manager.PauseToggleRequested -= TryTogglePause;
            manager.PauseToggleRequested += TryTogglePause;
            isPauseInputSubscribed = true;
        }
        else
        {
            manager.PauseToggleRequested -= TryTogglePause;
            isPauseInputSubscribed = false;
        }
    }

    private void TryTogglePause()
    {
        if ((dialogueUI != null && dialogueUI.gameObject.activeSelf) ||
            (minimap != null && minimap.activeSelf))
        {
            return;
        }

        if ((settingPanel != null && settingPanel.activeSelf) ||
            (controlSettingPanel != null && controlSettingPanel.activeSelf) ||
            (audioSettingPanel != null && audioSettingPanel.activeSelf))
        {
            return;
        }
        if (weaponDescriptionUI != null && weaponDescriptionUI.gameObject.activeSelf)
        {
            weaponDescriptionUI.gameObject.SetActive(false);
            return;
        }

        if (quitImage != null && quitImage.activeSelf)
        {
            quitImage.SetActive(false);
            return;
        }

        if (pausePanel != null && pausePanel.activeSelf)
        {
            ExitSetting();
            return;
        }

        OpenPause();
    }

    public void OpenPause()
    {
        pausePanel?.SetActive(true);
        Time.timeScale = 0;
        InputStateManager.Instance?.ChangeState(InputStateType.Pause);
    }

    public void ExitSetting()
    {
        pausePanel?.SetActive(false);
        Time.timeScale = 1f;
        InputStateManager.Instance?.ChangeState(InputStateType.Gameplay);
    }

    public void ClickContinue()
    {
        ExitSetting();
    }

    public void ClickRestart()
    {
        pausePanel?.SetActive(false);
        Time.timeScale = 1f;
        InputStateManager.Instance?.ChangeState(InputStateType.Gameplay);
        GameManager.Instance?.TryLoadScene("Scene_Title");
    }

    public void ClickSetting()
    {
        ClickSettingUI(settingPanel);
    }

    public void ClickControlSetting()
    {
        ClickSettingUI(controlSettingPanel);
    }

    public void ClickAudioSetting()
    {
        ClickSettingUI(audioSettingPanel);
    }

    public void TryExitGame(bool isTry)
    {
        quitImage?.SetActive(isTry);
    }

    public void ClickExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void ClickSettingUI(GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogWarning("[UIManager] 열 설정 패널이 비어 있습니다.", this);
            return;
        }

        panel.SetActive(true);
        pausePanel?.SetActive(false);
    }

    // 설정 하위 패널을 닫고 Pause 메뉴로 복귀한다.
    public void OnPausePanel()
    {
        settingPanel?.SetActive(false);
        controlSettingPanel?.SetActive(false);
        audioSettingPanel?.SetActive(false);

        pausePanel?.SetActive(true);
        Time.timeScale = 0f;
        InputStateManager.Instance?.ChangeState(InputStateType.Pause);
    }

    public IEnumerator CloseSceneRoutine()
    {
        if (loadPanel == null)
        {
            Debug.LogWarning("[UIManager] LoadPanel 이 null 입니다.", this);
            yield break;
        }

        yield return loadPanel.FadeIn();
    }

    public IEnumerator OpenSceneRoutine()
    {
        if (loadPanel == null)
        {
            Debug.LogWarning("[UIManager] LoadPanel 이 null 입니다.", this);
            yield break;
        }

        yield return loadPanel.FadeOut();
    }


    public void CloseScene()
    {
        if (loadPanel == null)
        {
            Debug.LogWarning("[UIManager] LoadPanel 이 null 입니다.", this);
            return;
        }

        loadPanel.ShowImmediate();
    }

    public void Restart()
    {
        pausePanel?.SetActive(false);
        Time.timeScale = 1f;
        InputStateManager.Instance?.ChangeState(InputStateType.Gameplay);

#if UNITY_EDITOR
        SceneManager.LoadScene("GameStart");
#else
        Application.Quit();
        System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe"));
#endif
    }

    // 상호작용 UI를 표시하고 필요하면 대상 위치로 이동한다.
    public void CanInteraction(bool can, Transform target = null)
    {
        if (interactiveImage == null)
        {
            return;
        }

        if (Time.timeScale == 0)
        {
            interactiveImage.gameObject.SetActive(false);
            return;
        }

        interactiveImage.gameObject.SetActive(can);

        if (!can || target == null)
        {
            return;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return;
        }

        IInteractable interactable = target.GetComponent<IInteractable>();
        Transform interactive = interactable?.InteractivePos;

        Vector3 worldPosition = interactive != null
            ? interactive.position
            : target.position + new Vector3(0f, 50f, 0f);

        interactiveImage.position = mainCamera.WorldToScreenPoint(worldPosition);
    }

    public void OpenMinimap()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial") return;
        Time.timeScale = Time.timeScale == 1 ? 0 : 1;
        minimap?.SetActive(!minimap.activeSelf);
    }

    public void ShowWeaponDescription(WeaponBase weapon)
    {
        if (weapon == null || weapon.WeaponDefinition == null || weaponDescriptionUI == null) return;

        weaponDescriptionUI.gameObject.SetActive(true);
        weaponDescriptionUI.ViewDescription(weapon);
    }
}
