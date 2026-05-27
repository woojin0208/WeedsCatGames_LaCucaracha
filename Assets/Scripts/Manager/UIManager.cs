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

        inventoryUI.SetActive(true);
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
            interactiveImage.gameObject.SetActive(false);
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
        if (dialogueUI.gameObject.activeSelf || minimap.activeSelf) return;
        if (settingPanel.activeSelf || controlSettingPanel.activeSelf) return;
        if (weaponDescriptionUI != null && weaponDescriptionUI.gameObject.activeSelf)
        {
            weaponDescriptionUI.gameObject.SetActive(false);
            return;
        }

        if (quitImage.activeSelf)
        {
            quitImage.SetActive(false);
            return;
        }

        if (pausePanel.activeSelf)
        {
            ExitSetting();
            return;
        }

        OpenPause();
    }

    public void OpenPause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        InputStateManager.Instance?.ChangeState(InputStateType.Pause);
    }

    public void ExitSetting()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        InputStateManager.Instance?.ChangeState(InputStateType.Gameplay);
    }

    public void ClickContinue()
    {
        ClickSettingUI(pausePanel);
        Time.timeScale = 1f;
        InputStateManager.Instance?.ChangeState(InputStateType.Gameplay);
    }

    public void ClickRestart()
    {
        ClickSettingUI(pausePanel);
        Time.timeScale = 1f;
        InputStateManager.Instance?.ChangeState(InputStateType.Gameplay);
        GameManager.Instance.TryLoadScene("Scene_Title");
    }

    public void ClickSetting()
    {
        ClickSettingUI(settingPanel);
    }

    public void ClickControlSetting()
    {
        ClickSettingUI(controlSettingPanel);
    }

    public void TryExitGame(bool isTry)
    {
        quitImage.SetActive(isTry);
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
        panel.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void OnPausePanel()
    {
        pausePanel.SetActive(true);
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
        pausePanel.SetActive(false);
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
        if (Time.timeScale == 0)
        {
            interactiveImage.gameObject.SetActive(false);
            return;
        }
        interactiveImage.gameObject.SetActive(can);

        if (target != null)
        {
            Transform interactive = target.GetComponent<IInteractable>().InteractivePos;

            Vector3 screenPoint = interactive != null ?
                Camera.main.WorldToScreenPoint(interactive.position)
                : Camera.main.WorldToScreenPoint(target.position + new Vector3(0, 50, 0)); // interactive UI 표시 offset

            interactiveImage.position = screenPoint;
        }
    }

    public void OpenMinimap()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial") return;
        Time.timeScale = Time.timeScale == 1 ? 0 : 1;
        minimap.SetActive(!minimap.activeSelf);
    }

    public void ShowWeaponDescription(WeaponBase weapon)
    {
        if (weapon.WeaponDefinition == null) return;

        weaponDescriptionUI.gameObject.SetActive(true);
        weaponDescriptionUI.ViewDescription(weapon);
    }
}
