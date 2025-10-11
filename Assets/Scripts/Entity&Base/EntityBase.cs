using System;
using System.Collections;
using UnityEngine;

public class EntityBase : MonoBehaviour
{
    [SerializeField]
    public EntityStat stats;
    private EntityStat Stats => stats;
    private bool isDead => Stats.HPStat != null && Mathf.Approximately(Stats.HPStat.Value, 0f);

    public Coroutine slowCoroutine { get; set; }

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

    public IEnumerator OnSlowEffect(float duration, float rate)
    {
        float bonusSpeed = stats.GetStat(StatType.MoveSpeed).DefaultValue * (rate / 100f);
        try
        {
            stats.GetStat(StatType.MoveSpeed).BonusValue -= bonusSpeed;
            yield return new WaitForSeconds(duration);
        }
        finally
        {
            stats.GetStat(StatType.MoveSpeed).BonusValue -= bonusSpeed; ;
        }
        
    }
}

// 스탯이 필요한 오브젝트 : player enemy
