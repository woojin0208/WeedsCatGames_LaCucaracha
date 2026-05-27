using UnityEngine;
using UnityEngine.SceneManagement;

// 현재 맵의 진입 지점 정보를 초기화한다.
public class MapManager : MonoBehaviour
{
    private Enterance[] enterances;

    private void Awake()
    {
        InitializeCurrentEnterance();
    }

    private void InitializeCurrentEnterance()
    {
        PlayerManager playerManager = PlayerManager.Instance;
        if (playerManager == null)
        {
            Debug.LogWarning("[MapManager] Player 가 null 입니다.", this);
            return;
        }

        enterances = GetComponentsInChildren<Enterance>(true);
        if (enterances == null || enterances.Length == 0)
        {
            Debug.LogWarning($"[MapManager] Enterance 가 비어 있습니다. scene : {SceneManager.GetActiveScene().name}", this);
            return;
        }

        for (int i = 0; i < enterances.Length; i++)
        {
            Enterance enterance = enterances[i];

            if (enterance.CurrentSpawnPoint == playerManager.CurrentSpawnPoint)
            {
                playerManager.CurrentEnterance = enterance;
                return;
            }
        }

        Debug.LogWarning($"[MapManager] CurrentSpawnPoint와 일치하는 Enterance가 없습니다. " +
            $"scene: {SceneManager.GetActiveScene().name}, spawnPoint: {playerManager.CurrentSpawnPoint}", this);
    }
}
