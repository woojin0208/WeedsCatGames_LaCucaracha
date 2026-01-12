using System;
using UnityEngine;

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

    /// <summary>
    /// JumpCount -1 : Fall / 2 : Null / 1 : Jump1 / 0 : Jump2
    /// </summary>
    /// 
    /// <param name="jump"></param>
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
