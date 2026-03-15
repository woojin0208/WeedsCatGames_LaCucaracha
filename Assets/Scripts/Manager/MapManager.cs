using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// 현재 맵의 진입 지점 정보를 초기화한다.
public class MapManager : MonoBehaviour
{
    private Enterance[] enterances;
    private int[] spawnPoints;

    private void Awake()
    {
        PlayerManager playerManger = PlayerManager.Instance;

        enterances = GetComponentsInChildren<Enterance>();

        // 현재 스폰 포인트와 일치하는 진입 지점을 찾는다.
        for (int i = 0; i < enterances.Length; i++)
        {
            if (enterances[i].CurrentSpawnPoint == playerManger.CurrentSpawnPoint)
            {
                playerManger.CurrentEnterance = enterances[i];

                Debug.Log(enterances[i].transform.position + enterances[i].transform.gameObject.name);
                break;
            }
        }

        string MapName = SceneManager.GetActiveScene().name;
        Debug.Log(MapName);
    }
}
