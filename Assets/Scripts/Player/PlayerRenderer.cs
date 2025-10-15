using UnityEngine;

public class PlayerRenderer : AnimationBase
{
    private PlayerAttack playerAttack;

    [SerializeField] private Color pipeWarpColor;
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

    public void WallClingAnim(bool isCling, float xDir = 0)
    {
        animator.SetBool("OnCling", isCling);

        Vector3 xScale = transform.localScale;
        xScale.x = xDir;
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
        animator.SetTrigger("OnThrow");
    }

    public void PipeWarpAnim(bool isStart, Vector3 pipePoint)
    {
        entityRenderer.color = isStart ? pipeWarpColor : originColor;

        Vector3 xScale = transform.position.x > pipePoint.x ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);

        animator.SetBool("OnPipe", isStart);
    }

    public bool GetDirection()
    {
        return transform.localScale.x == 1 ? false : true;
    }
}
