using UnityEngine;

// 고정형 보스의 머리와 몸통 애니메이션을 동기화한다.
public class AnchoredBossRenderer : AnimationBase
{
    [Header("Main (Head)")]
    [SerializeField] private Animator headAnimator;
    [SerializeField] private SpriteRenderer headRenderer;

    [Header("Sub (Body)")]
    [SerializeField] private Animator bodyAnimator;

    protected override void Awake()
    {
        base.Awake();

        // 머리 기준으로 기본 애니메이터와 렌더러를 연결한다.
        if (headAnimator != null)
            animator = headAnimator;
        if (headRenderer != null)
            entityRenderer = headRenderer;

        originColor = entityRenderer.color;
    }

    // 피격 시 머리와 몸통의 피격 애니메이션을 함께 실행한다.
    public override void TakeDamaged()
    {
        base.TakeDamaged();

        headAnimator?.SetTrigger("DamagedTrigger");
        bodyAnimator?.SetTrigger("DamagedTrigger");
    }

    // 공격 번호를 머리와 몸통 애니메이터에 동시에 전달한다.
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

    // 사망 시 머리와 몸통 애니메이션을 함께 종료 상태로 전환한다.
    public override void DieAnim()
    {
        base.DieAnim();

        headAnimator?.SetBool("IsDie", true);
        bodyAnimator?.SetBool("IsDie", true);
    }
}