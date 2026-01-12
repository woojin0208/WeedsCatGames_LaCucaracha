
using UnityEngine;

public class AnchoredBossRenderer : AnimationBase
{
    [Header("Main (Head)")]
    [SerializeField] private Animator headAnimator;           // 메인 Animator (Head)
    [SerializeField] private SpriteRenderer headRenderer;     // 메인 SpriteRenderer (Head)

    [Header("Sub (Body)")]
    [SerializeField] private Animator bodyAnimator;           // 서브 Animator (Body)

    protected override void Awake()
    {
        
        base.Awake();

        // Head 기준으로 렌더러 및 애니메이터 지정
        if (headAnimator != null)
            animator = headAnimator;
        if (headRenderer != null)
            entityRenderer = headRenderer;

        originColor = entityRenderer.color;
    }

    /// <summary>
    /// 피격 처리 - Head만 색상 반짝임 / Body도 애니메이션 공유
    /// </summary>
    public override void TakeDamaged()
    {
        base.TakeDamaged();

        headAnimator?.SetTrigger("DamagedTrigger");
        bodyAnimator?.SetTrigger("DamagedTrigger");
    }

    /// <summary>
    /// 공격 애니메이션 - Head / Body 모두 동일 파라미터 전달
    /// </summary>
    public override void AttackAnim(int attackNum)
    {
        base.AttackAnim(attackNum);

        headAnimator?.SetInteger("AttackNum", attackNum);
        bodyAnimator?.SetInteger("AttackNum", attackNum);

        Invoke(nameof(AttackEnd), 1.01f);
    }

    private void AttackEnd()
    {
        headAnimator?.SetInteger("AttackNum", 0);
        bodyAnimator?.SetInteger("AttackNum", 0);
    }
    /// <summary>
    /// 사망 애니메이션 - Head / Body 동기화
    /// </summary>
    public override void DieAnim()
    {
        base.DieAnim();

        headAnimator?.SetBool("IsDie", true);
        bodyAnimator?.SetBool("IsDie", true);
    }

}
