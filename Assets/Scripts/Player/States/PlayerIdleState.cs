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
    public bool CanClingWall { get; } = true;

    public void EnterState(PlayerController playerController)
    {
        playerController.Move.Idle(0);
        playerController.Anim.Idle();
    }

    public void UpdateState(PlayerController playerController)
    {
        
        bool grounded = playerController.Move.IsGrounded;

        if (wasGrounded && !grounded && !playerController.Input.JumpPressed)
        {
            playerController.ChangeState(new PlayerJumpState(true));
            return;
        }
        wasGrounded = grounded;

        playerController.Anim.Idle();
        //Debug.Log(h);
        /*
        if (Mathf.Abs(h) > 0.01f)
        {
            playerController.ChangeState(new PlayerWalkState());
            return;
        }

        if (playerController.Input.JumpPressed && playerController.Move.RemainingJumps > 0 )
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
    }
    public void ExitState(PlayerController playerController)
    {

    }
}
