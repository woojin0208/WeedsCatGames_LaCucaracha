using UnityEngine;

// 씬 전환용 입장 지점의 공통 정보를 보관한다.
public class Enterance : MonoBehaviour
{
    [Header("현재 입장 지점 정보")]
    [SerializeField] private string currentArea = "";
    [field: SerializeField] public int CurrentSpawnPoint { get; private set; } = 0;

    [Header("다음 입장 지점 정보")]
    [SerializeField] protected string nextArea = "";
    [field: SerializeField] public int NextSpawnPoint { get; private set; } = 0;

    [SerializeField] private AudioSource audioSource;
    protected virtual void EnterArea(string sceneName, EnteranceType enterance = EnteranceType.Normal)
    {
        if (sceneName == "ExitGame")
        {
            UIManager.Instance.TryExitGame(true);
            return;
        }

        if (audioSource != null) audioSource.Play();
        PlayerManager.Instance.SetCurrentScene(sceneName, NextSpawnPoint);

        GameManager.Instance.TryLoadScene(nextArea, enterance);
    }
}

// 입장 지점의 동작 유형을 정의한다.
public enum EnteranceType { Normal, Auto, Interactable, Guard, Pipe, NPC }