using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    private Enterance[] enterances;
    private int[] spawnPoints;

    private void Awake()
    {
        PlayerManager playerManger = PlayerManager.Instance;
        
        enterances = GetComponentsInChildren<Enterance>();

        // 현재 Enterance PM에 저장
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
