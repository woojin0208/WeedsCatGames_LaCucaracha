using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : IPlayerState
{
    public bool CanAttack { get; } = true;
    public bool CanJump { get; } = true;
    public bool CanDash { get; } = true;
    public bool CanWalk { get; } = false;
    public bool CanLadder { get; } = true;
    public bool CanClingWall { get; } = true;
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
        //Debug.Log(xVelocity);

        if (wasGrounded && !grounded && !playerController.Input.JumpPressed)
        {
            playerController.ChangeState(new PlayerJumpState(true));
            return;
        }

        playerController.Anim.Walk(playerController.Move.HorizontalDirection);
        /*
        if (playerController.Input.JumpPressed && playerController.Move.RemainingJumps > 0)
        {
            playerController.ChangeState(new PlayerJumpState());
            return;
        }

        if (playerController.Input.DashPressed)
        {
            playerController.ChangeState(new PlayerDashState());
            return;
        }

        if (playerController.Input.AttackPressed)
        {
            playerController.ChangeState(new PlayerAttackState());
            return;
        }
        */
        wasGrounded = grounded;
    }
    public void ExitState(PlayerController playerController) 
    { 

    }
}
