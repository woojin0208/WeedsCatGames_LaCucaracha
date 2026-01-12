using UnityEngine;

public class Enterance : MonoBehaviour
{
    [Header("현재 Enteracne 정보")]
    [SerializeField] private string currentArea = "";
    [field: SerializeField] public int CurrentSpawnPoint { get; private set; } = 0;

    [Header("다음 Enteracne 정보")]
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
public enum EnteranceType { Normal, Auto, Interactable, Guard, Pipe, NPC }