using System;
using UnityEngine;

public class EntityBase : MonoBehaviour
{
    [SerializeField]
    public EntityStat stats;
    private EntityStat Stats => stats;
    private bool isDead => Stats.HPStat != null && Mathf.Approximately(Stats.HPStat.Value, 0f);

    public event Action OnDiedAction;
    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;
        //Debug.Log($"Damage : {damage}");
        Stats.HPStat.DefaultValue -= damage;

        if (Mathf.Approximately(Stats.HPStat.DefaultValue, 0) || Stats.HPStat.DefaultValue < 0)
        {
            TryDie();
        }
    }

    protected virtual void TryDie()
    {
        //Destroy(this.gameObject, 1f);
    }

    public virtual void OnDied()
    {
        OnDiedAction?.Invoke();
        gameObject.SetActive(false);
    }
}

// 스탯이 필요한 오브젝트 : player enemy
