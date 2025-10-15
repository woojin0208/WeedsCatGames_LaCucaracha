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
    private Vector3 pipePoint;
    public PlayerPipeWarpState(bool isStart, Vector3 pipePoint)
    {
        isStartPipeWarp = isStart;
        this.pipePoint = pipePoint;
    }
    public void EnterState(PlayerController playerController)
    {
        playerController.Anim.PipeWarp(isStartPipeWarp, pipePoint);
    }

    public void UpdateState(PlayerController playerController)
    {

    }
    public void ExitState(PlayerController playerController)
    {

    }
}
