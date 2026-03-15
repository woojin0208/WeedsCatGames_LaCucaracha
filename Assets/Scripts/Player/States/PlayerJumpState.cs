using UnityEngine;

// PlayerJumpState 상태를 정의한다.
public class PlayerJumpState : IPlayerState
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
        // 점프 카운트를 소비하고 상승 속도를 적용한다.
        playerController.playerMovement.OnJump();
        playerController.Anim.Jump(playerController.Move.RemainingJumps);
    }

    public void UpdateState(PlayerController playerController)
    {
        // 공중에서 추가 점프 입력이 들어오면 이중 점프를 처리한다.
        if (playerController.Input.JumpPressed && playerController.Move.RemainingJumps > 0)
        {
            playerController.ChangeState(new PlayerJumpState());
            return;
        }

        // 상승이 끝나면 낙하 상태로 전환한다.
        if (playerController.Move.Velocity.y <= 0f)
        {
            playerController.ChangeState(new PlayerFallState());
            return;
        }

        if (playerController.Move.IsGrounded && Mathf.Abs(playerController.Move.Velocity.y) < 0.01f)
        {
            playerController.playerMovement.ResetJumpCount();
            playerController.ChangeState(new PlayerIdleState());
            return;
        }

        // 공중에서도 좌우 이동은 유지한다.
        playerController.Move.Move(playerController.Input.Horizontal);
    }

    public void ExitState(PlayerController playerController)
    {
        playerController.Anim.Jump(2);
    }
}