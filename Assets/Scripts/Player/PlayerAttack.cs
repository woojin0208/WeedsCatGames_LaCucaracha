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

    /*
    /// <summary>
    /// 공격 버튼 입력 시 호출
    /// </summary>
    public void Attack()
    {
        // 공격 중에 + 콤보 창(isComboWindow)도 아니면,
        // "버퍼"만 활성화하고 리턴 (즉, 모션 끝날 때 다음 타로 이어질 수 있게)
        //if (playerController.CurrentState == PlayerState.Jump || playerController.CurrentState == PlayerState.Dash || playerController.CurrentState == PlayerState.OnLadder) return;
        if (currentCombo == maxComboCount) return;
        if (isAttacking && !isComboWindow)
        {
            hasAttackBuffered = true;
            return;
        }

        // 실제 공격 시작 (혹은 다음 콤보 시작)
        currentCombo++;
        if (currentCombo > maxComboCount)
            currentCombo = maxComboCount;

        attackCollider.transform.localScale = playerRenderer.GetDirection() ? new Vector2(-1, 1) : new Vector2(1, 1);
        //playerController.ChangeState(PlayerState.Attack);
        //playerRenderer.AttackAnim(currentCombo);

        isAttacking = true;
        isComboWindow = false;  // “애니 끝날 때” 콤보 창을 열어줄 계획
        hasAttackBuffered = false;  // 새 공격 시작 시점에는 버퍼 초기화

        //attackCollider.enabled = true;

    }

    public void OnAttack()
    {
        Vector2 dir = playerRenderer.GetDirection() ? Vector2.right : Vector2.left;

        Vector2 origin = (Vector2)transform.position + dir * (attackRangeX * 0.5f);

        // 2) OverlapBoxAll 호출 (angle = 0)
        Collider2D[] hits = Physics2D.OverlapBoxAll(origin, new Vector2(attackRangeX, attackRangeY), 0f, LayerMask.GetMask("Enemy"));

        // 3) 적에게 데미지
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<EntityBase>(out var enemy))
                enemy.TakeDamage(CalcDamage());
        }

    }
    private void OnDrawGizmosSelected()
    {
        // 씬 뷰에 항상 보이게 하려면 이걸로 대체 가능
        Gizmos.color = Color.red;

        Vector2 dir = playerRenderer.GetDirection() ? Vector2.right : Vector2.left;

        Vector2 origin = (Vector2)transform.position + dir * (attackRangeX * 0.5f);

        Gizmos.DrawWireCube(origin, new Vector2(attackRangeX, attackRangeY));
    }

    private float CalcDamage()
    {
        float totalDamage = playerBase.stats.GetStat(StatType.AttackDamage).DefaultValue + playerBase.stats.GetStat(StatType.AttackDamage).BonusValue;

        // 3타째면(피니시)
        if (currentCombo == maxComboCount)
        {
            totalDamage += comboDamage;
        }

        return totalDamage;
    }

    /// <summary>
    /// 애니메이션 이벤트로 호출 (공격 모션 마지막 프레임에서)
    /// </summary>
    public void EndAttack()
    {
        // 현재 공격 모션 종료
        //playerController.ChangeState(PlayerState.Idle);
        attackCollider.enabled = false;
        isAttacking = false;

        // 공격 모션이 끝난 시점에 "콤보 창을 열어준다"
        // → 곧바로 다음 콤보를 이어갈 수 있게 (아주 짧은 순간)
        isComboWindow = true;

        // 버퍼가 있으면 즉시 다음 콤보로 넘어감
        if (hasAttackBuffered)
        {
            // 버퍼 소진
            hasAttackBuffered = false;

            // 바로 다음 콤보 실행
            Attack();
        }
        else
        {
            // 버퍼가 없다면, 즉시 콤보 리셋 → Idle 복귀
            ResetCombo();
        }
    }

    /// <summary>
    /// 콤보 리셋
    /// </summary>
    private void ResetCombo()
    {
        currentCombo = 0;
        isAttacking = false;
        isComboWindow = false;
        timeToNextInput = 0f;    // 미사용이지만, 초기화
        hasAttackBuffered = false;

        // Idle 애니메이션으로 복귀
        playerRenderer.AttackAnim(0);

    }

    /// <summary>
    /// 타격 콜라이더를 통한 데미지 판정
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EntityBase>(out var entity))
        {
            float totalDamage = playerBase.stats.GetStat(StatType.AttackDamage).DefaultValue + playerBase.stats.GetStat(StatType.AttackDamage).BonusValue;

            // 3타째면(피니시)
            if (currentCombo == maxComboCount)
            {
                totalDamage += comboDamage;
            }

            entity.TakeDamage(totalDamage);
        }
    }
    */


}
