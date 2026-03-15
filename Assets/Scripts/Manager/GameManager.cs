using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 전역 상태와 씬 전환을 관리한다.
public class GameManager : MonoBehaviour
{
    // 키 바인딩 데이터를 보관한다.
    public KeyBindingData keyBindingData;

    // 게임 정지 상태 변경을 전달한다.
    public event Action<bool> GameEventAction;

    // 씬 전환 시작을 알린다.
    public event Action SceneChangeAction;

    private bool sceneLoading;

    public int donationScore = 0;
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

    // 지정한 키 타입의 바인딩을 변경한다.
    public void KeyChanger(KeyType keyType, KeyCode keyCode)
    {
        keyBindingData.keys[(int)keyType] = keyCode;
    }

    // 씬 전환을 시작하고 입장 정보를 저장한다.
    public void TryLoadScene(string sceneName, EnteranceType enterance = EnteranceType.Normal)
    {
        if (sceneLoading) return;

        sceneLoading = true;
        CurrentEnterance = enterance;

        if (UIManager.Instance.gameObject.activeSelf) UIManager.Instance.CloseScene();

        StartCoroutine(LoadScene(sceneName));
    }

    // 전역 게임 이벤트를 발행한다.
    public void GameEvent(bool isStop)
    {
        GameEventAction?.Invoke(isStop);
    }

    // 씬 전환 연출 후 대상 씬을 로드한다.
    private IEnumerator LoadScene(string sceneName)
    {
        UIManager.Instance.gameObject.SetActive(true);
        SceneChangeAction?.Invoke();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
        sceneLoading = false;
    }

    // 게임 종료 요청을 처리한다.
    public void ExitGame()
    {
        Debug.Log("ExitGmae");
    }
}
