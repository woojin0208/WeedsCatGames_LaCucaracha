using UnityEngine;
using System;

// 플레이어 공격 판정과 무기 사용을 처리한다.
public class PlayerAttack : MonoBehaviour
{
    private BoxCollider2D attackCollider;
    [SerializeField] private int maxComboCount = 3;
    [SerializeField] private float attackRangeX = 1.0f;
    [SerializeField] private float attackRangeY = 0.5f;
    [SerializeField] private float comboBonusDamage = 4;

    private PlayerRenderer playerRenderer;
    private PlayerBase playerBase;
    private int comboStep;

    public event Action OnAttackHit;
    public event Action OnAttackEnd;

    private void Awake()
    {
        attackCollider = GetComponent<BoxCollider2D>();
        playerRenderer = GetComponentInParent<PlayerRenderer>();
        playerBase = GetComponentInParent<PlayerBase>();
        attackCollider.enabled = false;
    }

    public void InitStats(float attackDamage)
    {
    }

    // 현재 콤보 단계에 맞는 공격 애니메이션과 판정을 시작한다.
    public void PerformAttack(int step)
    {
        comboStep = Mathf.Clamp(step, 1, maxComboCount);

        bool isLeft = playerRenderer.IsLeft;
        attackCollider.transform.localScale = isLeft ? new Vector2(-1, 1) : Vector2.one;

        attackCollider.enabled = true;
        playerRenderer.AttackAnim(comboStep);
    }

    // 애니메이션 타격 프레임에서 데미지 판정을 실행한다.
    public void OnAttackHitEvent()
    {
        attackCollider.enabled = false;
        OnAttackHit?.Invoke();
        DealDamage();
    }

    // 애니메이션 종료 프레임에서 공격 종료를 알린다.
    public void OnAttackEndEvent()
    {
        attackCollider.enabled = false;
        OnAttackEnd?.Invoke();
    }

    private void DealDamage()
    {
        Vector2 dir = playerRenderer.IsLeft ? Vector2.left : Vector2.right;
        Vector2 origin = (Vector2)transform.position + dir * (attackRangeX * 0.5f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            origin,
            new Vector2(attackRangeX, attackRangeY),
            0f,
            LayerMask.GetMask("Enemy")
        );

        float baseDamage = playerBase.stats.GetStat(StatType.AttackDamage).DefaultValue
                         + playerBase.stats.GetStat(StatType.AttackDamage).BonusValue;
        if (comboStep == maxComboCount)
            baseDamage += comboBonusDamage;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<EntityBase>(out var enemy))
                enemy.TakeDamage(baseDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerRenderer == null) return;
        Gizmos.color = Color.red;

        Vector2 dir = playerRenderer.IsLeft ? Vector2.left : Vector2.right;
        Vector2 origin = (Vector2)transform.position + dir * (attackRangeX * 0.5f);

        Gizmos.DrawWireCube(origin, new Vector2(attackRangeX, attackRangeY));
    }
}