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
    [SerializeField] private GameObject loadPanel;
    [SerializeField] private RectTransform interactiveImage;
    [SerializeField] private GameObject minimap;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private GameObject quitImage;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private WeaponDescriptionUI weaponDescriptionUI;

    public bool canViewVideo = false;

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
        loadPanel.SetActive(false);
        inventoryUI.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (dialogueUI.gameObject.activeSelf || minimap.activeSelf) return;
            if (settingPanel.activeSelf || controlSettingPanel.activeSelf) return;
            if (quitImage.activeSelf)
            {
                quitImage.SetActive(false);
                return;
            }
            if (pausePanel.activeSelf)
            {
                ExitSetting();
            }
            else OpenPause();
        }

        if (Time.timeScale == 0)
        {
            interactiveImage.gameObject.SetActive(false);
            return;
        }
    }

    public void OpenPause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ExitSetting()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ClickContinue()
    {
        ClickSettingUI(pausePanel);
        Time.timeScale = 1f;
    }

    public void ClickRestart()
    {
        ClickSettingUI(pausePanel);
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
    }

    public void CloseScene()
    {
        loadPanel.SetActive(true);
    }

    public void Restart()
    {
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
                : Camera.main.WorldToScreenPoint(target.position + new Vector3(0, 50, 0));

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
