using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // ³«ÇÏ
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
