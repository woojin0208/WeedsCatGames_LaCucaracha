using System;
using System.Collections;
using UnityEngine;

// 체력과 공통 상태 이상 처리를 제공하는 엔티티 기반 클래스다.
public class EntityBase : MonoBehaviour
{
    [SerializeField]
    public EntityStat stats;
    private EntityStat Stats => stats;
    private bool isDead => Stats.HPStat != null && Mathf.Approximately(Stats.HPStat.Value, 0f);

    public Coroutine slowCoroutine { get; set; }
    private MovementBase movement;
    
    public event Action OnDiedAction;

    protected virtual void Awake()
    {
        movement = GetComponent<MovementBase>();
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;
        Stats.HPStat.DefaultValue -= damage;

        if (Mathf.Approximately(Stats.HPStat.DefaultValue, 0) || Stats.HPStat.DefaultValue < 0)
        {
            TryDie();
        }
    }

    protected virtual void TryDie()
    {
    }

    public virtual void OnDied()
    {
        OnDiedAction?.Invoke();
        gameObject.SetActive(false);
    }

    public IEnumerator OnSlowEffect(float duration, float rate)
    {
        Stat moveSpeed = stats.GetStat(StatType.MoveSpeed);
        float bonusSpeed = moveSpeed.DefaultValue * (rate / 100f);
        try
        {
            moveSpeed.BonusValue -= bonusSpeed;
            movement?.SetMoveSpeed(moveSpeed.Value);
            yield return new WaitForSeconds(duration);
        }
        finally
        {
            moveSpeed.BonusValue += bonusSpeed;
            movement?.SetMoveSpeed(moveSpeed.Value);
        }
        
    }
}
