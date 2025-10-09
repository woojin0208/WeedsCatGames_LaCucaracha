using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerRenderer : AnimationBase
{
    private PlayerAttack playerAttack;

    protected override void Awake()
    {
        base.Awake();
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

    public void ClingWallAnim(bool isCling)
    {

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
        animator.SetTrigger("OnThrow");
    }
    public bool GetDirection()
    {
        return transform.localScale.x == 1 ? false : true;
    }
}
