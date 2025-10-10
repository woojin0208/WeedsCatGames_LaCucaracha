using UnityEngine;
using System;

public class PlayerAttack : MonoBehaviour
{
    //private PlayerController playerController;
    //private PlayerRenderer playerRenderer;
    //private BoxCollider2D attackCollider;
    //private PlayerBase playerBase;
    //private float attackDamage;
    //private float comboBonusDamage = 4;
    
    private BoxCollider2D attackCollider;
    [SerializeField] private int maxComboCount = 3;  // 3타 콤보 (3번째가 피니시)
    //[SerializeField] private float comboTimer = 1f;  // 더 이상 사용하지 않지만, 삭제/이름 변경 없이 둠
    [SerializeField] private float attackRangeX = 1.0f;
    [SerializeField] private float attackRangeY = 0.5f; 
    [SerializeField] private float comboBonusDamage = 4;


    private PlayerRenderer playerRenderer;
    private PlayerBase playerBase;
    private int comboStep;
    //private float timeToNextInput = 0f;             // 사용하지 않음 (유지하되 로직 제거)

    //private bool isAttacking = false;
    //private bool isComboWindow = false;

    // 공격 중 버튼을 다시 눌렀을 때, 다음 콤보를 예약하는 버퍼
    //private bool hasAttackBuffered = false;

    public event Action OnAttackHit;
    public event Action OnAttackEnd;


    private void Awake()
    {
        //playerController = GetComponentInParent<PlayerController>();
        attackCollider = GetComponent<BoxCollider2D>();
        playerRenderer = GetComponentInParent<PlayerRenderer>();
        playerBase = GetComponentInParent<PlayerBase>();
        attackCollider.enabled = false;
    }

    public void InitStats(float attackDamage)
    {
        //this.attackDamage = attackDamage;
    }

    /// <summary>
    /// Called by the attack state to perform the attack for the given combo step.
    /// </summary>
    public void PerformAttack(int step)
    {
        comboStep = Mathf.Clamp(step, 1, maxComboCount);

        // Flip collider based on facing direction
        bool isLeft = playerRenderer.IsLeft;
        attackCollider.transform.localScale = isLeft ? new Vector2(-1, 1) : Vector2.one;

        attackCollider.enabled = true;
        playerRenderer.AttackAnim(comboStep);
    }

    /// <summary>
    /// Called via Animation Event at the hit frame to deal damage.
    /// </summary>
    public void OnAttackHitEvent()
    {
        attackCollider.enabled = false;
        OnAttackHit?.Invoke();
        DealDamage();
    }

    /// <summary>
    /// Called via Animation Event at the end frame to signal state transition.
    /// </summary>
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
        // 씬 뷰에 항상 보이게 하려면 이걸로 대체 가능
        if (playerRenderer == null) return;
        Gizmos.color = Color.red;

        Vector2 dir = playerRenderer.IsLeft ? Vector2.left : Vector2.right;

        Vector2 origin = (Vector2)transform.position + dir * (attackRangeX * 0.5f);

        Gizmos.DrawWireCube(origin, new Vector2(attackRangeX, attackRangeY));
    }
}
