using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적 사망 시 드롭 아이템 생성을 처리한다.
public class EnemyDropItem : MonoBehaviour
{
    [SerializeField] private GameObject[] items;
    [SerializeField] private float dropProbability = 1.0f;
    
    private EnemyBase enemyBase;

    private void Awake()
    {
        enemyBase = GetComponent<EnemyBase>();

        enemyBase.OnDiedAction += TryDropItem;
    }

    private void TryDropItem()
    {
        Debug.Log("Try Drop");
        bool isDrop = Random.Range(0f, 1f) <= dropProbability;

        int idx = isDrop ? Random.Range(0, items.Length) : -1;

        Debug.Log($"isDrop = {isDrop} \n idx = {idx} {items[idx].name}");

        if (idx >= 0)
        {
            GameObject itemClone = Instantiate(items[idx]);
            itemClone.transform.position = transform.position;
            itemClone.transform.SetParent(null);
        }
    }

    private void OnDisable()
    {
    }
}