using System;
using UnityEngine;

// 플레이어 애니메이션과 렌더링을 담당한다.
public class PlayerRenderer : AnimationBase
{
    private PlayerAttack playerAttack;
    private VFXPlayer vfxPlayer;

    [SerializeField] private int throwVfxIndex = 3;

    protected override void Awake()
    {
        base.Awake();
        vfxPlayer = GetComponent<VFXPlayer>();
        playerAttack = GetComponentInChildren<PlayerAttack>();
    }

    // JumpCount 파라미터 값으로 점프 단계와 낙하 상태를 구분한다.
    public void JumpAnim(int jump)
    {
        if (animator == null) return;

        animator.SetInteger(AnimatorParams.JumpCount, jump);
    }

    public void OnAttackTiming()
    {
        playerAttack?.OnAttackHitEvent();
    }
    public void EndAttackAnim()
    {
        playerAttack?.OnAttackEndEvent();
    }

    public void DashAnim()
    {
        if (animator == null) return;

        animator.SetTrigger(AnimatorParams.OnDashTrigger);
    }

    public void WallClingAnim(bool isCling, float xDir = 0)
    {
        if (animator != null)
        {
            animator.SetBool(AnimatorParams.OnCling, isCling);
        }

        Vector3 xScale = Vector3.one;
        if (xDir != 0) xScale.x = xDir;
        transform.localScale = xScale;
    }
    public void LadderAnim(bool isClimb)
    {
        IsLeft = true;

        Vector3 xScale = transform.localScale;
        xScale.x = IsLeft ? 1 : -1;
        transform.localScale = xScale;

        if (animator == null) return;

        animator.SetBool(AnimatorParams.OnLadder, isClimb);
    }

    public void ThrowAnim()
    {
        vfxPlayer?.StartVFX(throwVfxIndex);

        if (animator == null) return;

        animator.SetTrigger(AnimatorParams.OnThrow);
    }

    public void PipeWarpAnim(bool isStart, bool isLeft)
    {
        Vector3 xScale = transform.localScale;
        xScale.x = isLeft ? 1 : -1;
        transform.localScale = xScale;

        if (isStart)
        {
            if (animator == null) return;

            animator.SetTrigger(AnimatorParams.OnPipe);
        }
        else
        {
            xScale.x *= -1;

            if (animator == null) return;

            animator.SetTrigger(AnimatorParams.ReversePipe);
        }
    }

    public bool GetDirection()
    {
        return transform.localScale.x == 1 ? false : true;
    }
}
