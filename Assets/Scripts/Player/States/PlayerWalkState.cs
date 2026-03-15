using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerWalkState 상태를 정의한다.
public class PlayerWalkState : IPlayerState
{
    public bool CanAttack { get; } = true;
    public bool CanJump { get; } = true;
    public bool CanDash { get; } = true;
    public bool CanWalk { get; } = false;
    public bool CanLadder { get; } = true;
    public bool CanClingWall { get; } = false;
    public bool CanPipeWarp { get; } = true;
    private bool wasGrounded;

    public void EnterState(PlayerController playerController)
    {
        wasGrounded = playerController.Move.IsGrounded;
        playerController.Anim.Walk(playerController.Move.HorizontalDirection);
    }

    public void UpdateState(PlayerController playerController)
    {
        bool grounded = playerController.Move.IsGrounded;

        float xInput = playerController.Input.Horizontal;
        playerController.Move.Move(xInput);

        float xVelocity = playerController.Move.Velocity.x;
        if (Mathf.Abs(xVelocity) < 0.01f)
        {
            playerController.ChangeState(new PlayerIdleState());
            return;
        }

        playerController.Anim.Walk(xVelocity);

        if (wasGrounded && !grounded && !playerController.Input.JumpPressed)
        {
            playerController.ChangeState(new PlayerFallState());
            return;
        }

        playerController.Anim.Walk(playerController.Move.HorizontalDirection);
        wasGrounded = grounded;
    }

    public void ExitState(PlayerController playerController)
    {
    }
}