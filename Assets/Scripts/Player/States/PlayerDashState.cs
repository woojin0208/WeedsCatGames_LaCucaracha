using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerDashState 상태를 정의한다.
public class PlayerDashState : IPlayerState
{
    public bool CanAttack { get; } = false;
    public bool CanJump { get; } = false;
    public bool CanDash { get; } = false;
    public bool CanWalk { get; } = false;
    public bool CanLadder { get; } = false;
    public bool CanClingWall { get; } = false;
    public bool CanPipeWarp { get; } = false;
    private bool started;

    public void EnterState(PlayerController playerController)
    {
        if (playerController.Move.IsDashing || playerController.playerMovement.currentDashCooldown > 0) return;
        playerController.Anim.Dash();
        playerController.Move.Dash(playerController.Input.Horizontal);
    }

    public void UpdateState(PlayerController playerController)
    {
        // 대시가 끝나면 대기 상태로 복귀한다.
        if (!playerController.Move.IsDashing)
        {
            playerController.ChangeState(new PlayerIdleState());
        }
    }

    public void ExitState(PlayerController playerController) { }
}