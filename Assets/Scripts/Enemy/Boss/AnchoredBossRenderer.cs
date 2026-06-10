using UnityEngine;

// 고정형 보스의 머리와 몸통 애니메이션을 동기화한다.
public class AnchoredBossRenderer : AnimationBase
{
    private const float AttackResetDelay = 1.01f;
    private const int IdleAttackNum = 0;

    [Header("Main (Head)")]
    [SerializeField] private Animator headAnimator;
    [SerializeField] private SpriteRenderer headRenderer;

    [Header("Sub (Body)")]
    [SerializeField] private Animator bodyAnimator;

    protected override void Awake()
    {
        base.Awake();

        if (headAnimator != null)
            animator = headAnimator;

        if (headRenderer != null)
        {
            entityRenderer = headRenderer;
            originColor = entityRenderer.color;
        }
    }

    // 피격 시 머리와 몸통의 피격 애니메이션을 함께 실행한다.
    public override void TakeDamaged()
    {
        base.TakeDamaged();
    }

    // 공격 번호를 머리와 몸통 애니메이터에 동시에 전달한다.
    public override void AttackAnim(int attackNum)
    {
        headAnimator?.SetInteger(AnimatorParams.AttackNum, attackNum);
        bodyAnimator?.SetInteger(AnimatorParams.AttackNum, attackNum);

        CancelInvoke(nameof(AttackEnd));
        Invoke(nameof(AttackEnd), AttackResetDelay);
    }

    private void AttackEnd()
    {
        headAnimator?.SetInteger(AnimatorParams.AttackNum, IdleAttackNum);
        bodyAnimator?.SetInteger(AnimatorParams.AttackNum, IdleAttackNum);
    }

    public override void DieAnim()
    {
        CancelInvoke(nameof(AttackEnd));

        headAnimator?.SetBool(AnimatorParams.IsDie, true);
        bodyAnimator?.SetBool(AnimatorParams.IsDie, true);
    }
}