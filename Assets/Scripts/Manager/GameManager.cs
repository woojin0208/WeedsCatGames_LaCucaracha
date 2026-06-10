using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 전역 상태와 씬 전환을 관리한다.
public class GameManager : MonoBehaviour
{
    // 게임 정지 상태 변경을 전달한다.
    public event Action<bool> GameEventAction;

    // 씬 전환 시작을 알린다.
    public event Action SceneChangeAction;

    private bool sceneLoading;

    public bool Stage2CutScene = false;

    // 현재 씬에 진입할 때 사용할 입장 타입을 보관한다.
    public EnteranceType CurrentEnterance { get; private set; }

    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

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

    // 씬 전환을 시작하고 입장 정보를 저장한다.
    public bool TryLoadScene(string sceneName, EnteranceType enterance = EnteranceType.Normal)
    {
        if (sceneLoading)
        {
            Debug.LogWarning($"[GameManager] 이미 Scene 전환 중입니다. scene: {sceneName}", this);
            return false;
        }

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("[GameManager] sceneName 이 비어 있습니다.", this);
            return false;
        }

        sceneLoading = true;
        CurrentEnterance = enterance;

        StartCoroutine(LoadScene(sceneName));
        return true;
    }

    // 전역 게임 이벤트를 발행한다.
    public void GameEvent(bool isStop)
    {
        GameEventAction?.Invoke(isStop);
    }

    // 씬 전환 연출 후 대상 씬을 로드한다.
    private IEnumerator LoadScene(string sceneName)
    {
        Time.timeScale = 1f;

        InputStateManager inputManager = InputStateManager.Instance;
        inputManager?.LockInput();

        UIManager uiManager= UIManager.Instance;
        if (uiManager != null)
        {
            yield return uiManager.CloseSceneRoutine();
        }

        SceneChangeAction?.Invoke();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        if (operation == null)
        {
            Debug.LogError($"[GameManager] Scene Load 에 실패 했습니다. sceneName : {sceneName}", this);
            inputManager?.UnlockInput();
            sceneLoading = false;
            yield break;
        }

        while (!operation.isDone)
        {
            yield return null;
        }

        if (uiManager != null)
        {
            yield return uiManager.OpenSceneRoutine();
        }

        inputManager?.UnlockInput();
        sceneLoading = false;
    }

    // 게임 종료 요청을 처리한다.
    public void ExitGame()
    {
        Debug.Log("ExitGmae");
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha0)) TryLoadScene("Stage12");
#endif
    }
}
