using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public KeyBindingData keyBindingData;

    public event Action<bool> GameEventAction;

    public event Action SceneChangeAction;

    private bool sceneLoading;

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

    public void KeyChanger(KeyType keyType, KeyCode keyCode)
    {
        keyBindingData.keys[(int)keyType] = keyCode;
        /*
        switch (keyType)
        {
            case KeyType.Attack:
                keyBindingData.keys[(int)keyType] = keyCode;
                break;
                
            case KeyType.Dash:
                keyBindingData.dashKey = keyCode;
                break;
            case KeyType.Jump:
                keyBindingData.jumpKey = keyCode;
                break;
            case KeyType.Up:
                keyBindingData.upKey = keyCode;
                break;
            case KeyType.Down:
                keyBindingData.downKey = keyCode;
                break;
            case KeyType.Left:
                keyBindingData.leftKey = keyCode;
                break;
            case KeyType.Right:
                keyBindingData.rightKey = keyCode;
                break;
        }
        */
    }

    public void TryLoadScene(string sceneName, EnteranceType enterance = EnteranceType.Normal)
    {
        if (sceneLoading) return;

        sceneLoading = true;

        CurrentEnterance = enterance;

        if (UIManager.Instance.gameObject.activeSelf) UIManager.Instance.CloseScene();

        StartCoroutine(LoadScene(sceneName));
    }

    public void GameEvent(bool isStop) // ƒ∆æ¿ µÓ¿« ¿Ã∫•∆Æ∑Œ, Enemy π◊ Player¿« «‡µø¿Ã ∏ÿ√„.
    {
        GameEventAction?.Invoke(isStop);
    }

    private IEnumerator LoadScene(string sceneName)
    {
        UIManager.Instance.gameObject.SetActive(true);
        SceneChangeAction?.Invoke();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
        sceneLoading = false;
    }

    public void ExitGame()
    {
        Debug.Log("ExitGmae");
    }
}
