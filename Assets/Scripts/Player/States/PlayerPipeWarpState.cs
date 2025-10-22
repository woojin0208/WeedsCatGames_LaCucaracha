using UnityEngine;

public class PlayerPipeWarpState : IPlayerState
{
    public bool CanAttack { get; } = false;
    public bool CanJump { get; } = false;
    public bool CanDash { get; } = false;
    public bool CanWalk { get; } = false;
    public bool CanLadder { get; } = false;
    public bool CanClingWall { get; } = false;
    public bool CanPipeWarp { get; } = false;

    private bool isStartPipeWarp;
    private bool isLeftStart;

    private float warpTime = 0.75f; // 나중에 event로 변경

    public PlayerPipeWarpState(bool isStart, bool isLeft)
    {
        isStartPipeWarp = isStart;
        this.isLeftStart = isLeft;
    }

    public void EnterState(PlayerController playerController)
    {
        playerController.Move.Stop(true);
        playerController.Anim.PipeWarp(isStartPipeWarp, isLeftStart);

        Debug.LogAssertion("Enter PipeWarp");
    }

    public void UpdateState(PlayerController playerController)
    {
        warpTime -= Time.deltaTime;

        if (warpTime <= 0)
        {
            playerController.Move.Stop(false);
            playerController.ChangeState(new PlayerIdleState());
        }
        else playerController.Move.Stop(true);
    }

    public void ExitState(PlayerController playerController)
    {
        playerController.Move.Stop(false);
        Debug.LogAssertion("Exit PipeWarp");
    }
}
