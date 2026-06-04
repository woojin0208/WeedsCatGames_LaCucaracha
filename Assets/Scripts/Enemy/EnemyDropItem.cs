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

        if (enemyBase == null)
        {
            Debug.LogWarning("[EnemyDropItem] EnemyBase 가 null 입니다.", this);
            return;
        }

        enemyBase.OnDiedAction += TryDropItem;
    }

    private void TryDropItem()
    {
        if (items == null || items.Length == 0)
        {
            Debug.LogWarning("[EnemyDropItem] 드랍 아이템 목록이 비어 있습니다.", this);
            return;
        }

        bool isDrop = Random.Range(0f, 1f) <= dropProbability;
        if (!isDrop) return;

        int index = Random.Range(0, items.Length);
        GameObject itemPrefab = items[index];

        if (itemPrefab == null)
        {
            Debug.LogWarning($"[EnemyDropItem] DropItem 이 null 입닏다. index : {index}", this);
            return;
        }

        GameObject itemClone = Instantiate(itemPrefab);
        itemClone.transform.position = transform.position;
        itemClone.transform.SetParent(null);
    }

    private void OnDisable()
    {
        if (enemyBase != null)
        {
            enemyBase.OnDiedAction -= TryDropItem;
        }
    }
}