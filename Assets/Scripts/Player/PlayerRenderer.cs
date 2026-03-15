using System;
using UnityEngine;

// 플레이어 애니메이션과 렌더링을 담당한다.
public class PlayerRenderer : AnimationBase
{
    private PlayerAttack playerAttack;
    private VFXPlayer vfxPlayer;
    [SerializeField] private Color pipeWarpColor;

    protected override void Awake()
    {
        base.Awake();
        vfxPlayer = GetComponent<VFXPlayer>();
        playerAttack = GetComponentInChildren<PlayerAttack>();
    }

    // JumpCount 파라미터 값으로 점프 단계와 낙하 상태를 구분한다.
    public void JumpAnim(int jump)
    {
        animator.SetInteger("JumpCount", jump);
    }

    public void OnAttactkTiming()
    {
        playerAttack.OnAttackHitEvent();
    }
    public void EndAttackAnim()
    {
        playerAttack.OnAttackEndEvent();
    }

    public void DashAnim()
    {
        animator.SetTrigger("OnDashTrigger");
    }

    public void WallClingAnim(bool isCling, float xDir = 0)
    {
        animator.SetBool("OnCling", isCling);

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

        animator.SetBool("OnLadder", isClimb);
    }

    public void ThrowAnim()
    {
        vfxPlayer.StartVFX(3);
        animator.SetTrigger("OnThrow");
    }

    public void PipeWarpAnim(bool isStart, bool isLeft)
    {
        Debug.Log($"{isLeft} = isLeft");
        Vector3 xScale = transform.localScale;
        xScale.x = isLeft ? 1 : -1;
        transform.localScale = xScale;

        if (isStart)
        {
            animator.SetTrigger("OnPipe");
        }
        else
        {
            xScale.x *= -1;
            animator.SetTrigger("ReversePipe");
        }
    }

    public bool GetDirection()
    {
        return transform.localScale.x == 1 ? false : true;
    }
}