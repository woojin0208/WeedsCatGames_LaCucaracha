using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerIdleState 상태를 정의한다.
public class PlayerIdleState : IPlayerState
{
    private bool wasGrounded = true;

    public bool CanAttack { get; } = true;
    public bool CanJump { get; } = true;
    public bool CanDash { get; } = false;
    public bool CanWalk { get; } = true;
    public bool CanLadder { get; } = true;
    public bool CanClingWall { get; } = false;
    public bool CanPipeWarp { get; } = true;

    public void EnterState(PlayerController playerController)
    {
        playerController.Move.Idle(0);
        playerController.Anim.Idle();
        wasGrounded = true;
    }

    public void UpdateState(PlayerController playerController)
    {
        bool grounded = playerController.Move.IsGrounded;

        // 바닥에서 떨어졌고 점프 입력이 아니면 낙하 상태로 전환한다.
        if (wasGrounded && !grounded && !playerController.Input.JumpPressed)
        {
            playerController.ChangeState(new PlayerFallState());
            return;
        }
        wasGrounded = grounded;

        playerController.Anim.Idle();
    }

    public void ExitState(PlayerController playerController)
    {
    }
}