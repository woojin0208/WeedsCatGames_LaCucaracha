using UnityEngine;

// PlayerFallState 상태를 정의한다.
public class PlayerFallState : IPlayerState
{
    public bool CanAttack => true;
    public bool CanJump => true;
    public bool CanDash => true;
    public bool CanWalk => false;
    public bool CanLadder => false;
    public bool CanClingWall => true;
    public bool CanPipeWarp => true;

    public void EnterState(PlayerController playerController)
    {
        playerController.Anim.Jump(-1);
    }

    public void UpdateState(PlayerController playerController)
    {
        if (playerController.Input.JumpPressed && playerController.Move.RemainingJumps > 0)
        {
            playerController.ChangeState(new PlayerJumpState());
            return;
        }

        if (playerController.Move.IsGrounded && Mathf.Abs(playerController.Move.Velocity.y) < 0.01f)
        {
            playerController.playerMovement.ResetJumpCount();
            playerController.ChangeState(new PlayerIdleState());
            return;
        }

        // 낙하 중에도 좌우 이동은 유지한다.
        playerController.Move.Move(playerController.Input.Horizontal);
    }

    public void ExitState(PlayerController playerController)
    {
        playerController.Anim.Jump(2);
    }
}