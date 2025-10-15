using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //Debug.Log(playerController.Move.IsDashing);
        // Once dash coroutine ends, return to Idle
        if (!playerController.Move.IsDashing)
        {
            playerController.ChangeState(new PlayerIdleState());
        }
    }

    public void ExitState(PlayerController playerController) { }
}
