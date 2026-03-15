using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerClimbLadderState 상태를 정의한다.
public class PlayerClimbLadderState : IPlayerState
{
    public bool CanAttack { get; } = false;
    public bool CanJump { get; } = false;
    public bool CanDash { get; } = false;
    public bool CanWalk { get; } = false;
    public bool CanLadder { get; } = false;
    public bool CanClingWall { get; } = false;
    public bool CanPipeWarp { get; } = false;
    private Vector2 startPos, endPos;

    public PlayerClimbLadderState(Vector2 start, Vector2 end)
    {
        startPos = start;
        endPos = end;
    }

    public void EnterState(PlayerController playerController)
    {
        Debug.Log(playerController.name);
        playerController.playerMovement.ClimbLadder(startPos, endPos);
        Debug.Log(startPos + " " + endPos);
        playerController.Anim.Ladder(true);
    }

    public void UpdateState(PlayerController playerController)
    {
        if (!playerController.playerMovement.IsClimbLadder) playerController.ChangeState(new PlayerIdleState());
    }

    public void ExitState(PlayerController playerController)
    {
        playerController.Anim.Ladder(false);
    }
}