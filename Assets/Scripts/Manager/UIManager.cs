using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get { return instance; }
    }

    [SerializeField]
    private GameObject pausePanel; // pause Image
    [SerializeField]
    private GameObject settingPanel;
    [SerializeField]
    private GameObject controlSettingPanel;

    [SerializeField]
    private GameObject loadPanel;

    [SerializeField]
    private RectTransform interactiveImage;

    [SerializeField]
    private GameObject minimap;
    [SerializeField]
    private DialogueUI dialogueUI;

    [SerializeField]
    private GameObject quitImage;

    [SerializeField]
    private GameObject inventoryUI;

    public bool canViewVideo = true;
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

        //Debug.Log("엥 왜 안킴");
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
        //GameManager.Instance.ExitGame();

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
        // 에디터에서는 도중에 "재실행" 못하니까 그냥 StartScene 로드
        SceneManager.LoadScene("StartScene");
#else
        // 빌드된 게임에서는 프로세스 자체를 껐다 켜는 게 가장 확실
        Application.Quit();
        System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe"));
#endif
    }
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
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.position);
            screenPoint.y += 50;
            interactiveImage.position = screenPoint;
        }

        //if (target != null) Debug.Log(target.position);

    }

    public void OpenMinimap()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial") return;
        Time.timeScale = Time.timeScale == 1 ? 0 : 1;
        minimap.SetActive(!minimap.activeSelf);

    }
}
