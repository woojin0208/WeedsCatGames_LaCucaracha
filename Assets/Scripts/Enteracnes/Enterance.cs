using UnityEngine;

// 씬 전환용 입장 지점의 공통 정보를 보관한다.
public class Enterance : MonoBehaviour
{
    [SerializeField] private string currentArea = "";
    [field: SerializeField] public int CurrentSpawnPoint { get; private set; } = 0;

    [SerializeField] protected string nextArea = "";
    [field: SerializeField] public int NextSpawnPoint { get; private set; } = 0;

    [SerializeField] private AudioSource audioSource;

    protected virtual bool EnterArea(EnteranceType enterance = EnteranceType.Normal)
    {
        if (string.IsNullOrWhiteSpace(nextArea))
        {
            Debug.LogWarning("[Enterance] nextArea 가 설정되지 않았습니다.", this);
            return false;
        }

        if (nextArea == "ExitGame")
        {
            UIManager.Instance?.TryExitGame(true);
            return true;
        }

        PlayerManager playerManager = PlayerManager.Instance;
        if (playerManager == null)
        {
            Debug.LogWarning("[Enterance] PlayerManager 가 null 입니다.", this);
            return false;
        }

        GameManager gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogWarning("[Enterance] GameManager 가 null 입니다.", this);
            return false;
        }

        PlayEnterSound();

        playerManager.SetCurrentScene(nextArea, NextSpawnPoint);
        return gameManager.TryLoadScene(nextArea, enterance);
    }

    private void PlayEnterSound()
    {
        if (audioSource == null) return;
        if (audioSource.clip == null) return;

        audioSource.Play();
    }
}


// 입장 지점의 동작 유형을 정의한다.
public enum EnteranceType
{
    Normal,
    Auto,
    Interactable,
    Guard,
    Pipe,
    NPC
}