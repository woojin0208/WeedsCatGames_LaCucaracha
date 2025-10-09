using System;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private Enterance[] enterances;
    private int[] spawnPoints;

    private void Awake()
    {
        PlayerManager playerManger = PlayerManager.Instance;
        enterances = GetComponentsInChildren<Enterance>();

        for (int i = 0; i < enterances.Length; i++)
        {
            if (enterances[i].CurrentSpawnPoint == playerManger.CurrentEnterance)
            {
                playerManger.CurrentSpawnPoint = enterances[i].transform.position;
                
                Debug.Log(enterances[i].transform.position + enterances[i].transform.gameObject.name);

                break;
            }
        }

    }
}
