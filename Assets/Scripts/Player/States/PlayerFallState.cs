using UnityEngine;
public class PlayerFallState : IPlayerState
{
    public bool CanAttack => true;
    public bool CanJump => true;      // 낙하 중 이중점프 허용
    public bool CanDash => true;
    public bool CanWalk => false;
    public bool CanLadder => false;
    public bool CanClingWall => true; // 벽매달림 허용
    public bool CanPipeWarp => true;

    public void EnterState(PlayerController playerController)
    {
        playerController.Anim.Jump(-1); // 낙하 애니
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

        // 공중 좌우 이동
        playerController.Move.Move(playerController.Input.Horizontal);
    }

    public void ExitState(PlayerController playerController)
    {
        playerController.Anim.Jump(2);
    }
}