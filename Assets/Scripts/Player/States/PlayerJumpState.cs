using UnityEngine;

public class PlayerJumpState : IPlayerState
{
    private readonly bool isFalling;

    public bool CanAttack { get; } = true;
    public bool CanJump { get; } = true;
    public bool CanDash { get; } = true;
    public bool CanWalk { get; } = false;
    public bool CanLadder { get; } = false;
    public bool CanClingWall { get; } = true;

    /// <summary>
    ///  생성자 메서드
    /// </summary>
    /// <param name="isFalling"></param>
    public PlayerJumpState(bool isFalling = false)
    {
        this.isFalling = isFalling;

    }


    public void EnterState(PlayerController playerController)
    {
        if (isFalling)
        {
            // 낙하 시: 점프 횟수만 하나 깎고 바로 낙하 애니메이션
            playerController.playerMovement.DecrementJumpCount();
            //Debug.Log("점프 없이 낙하");
            playerController.Anim.Jump(-1);
        }
        else
        {
            // 점프 입력 시
            playerController.playerMovement.OnJump();
            playerController.Anim.Jump(playerController.Move.RemainingJumps);
        }
    }

    public void UpdateState(PlayerController playerController)
    {
        // 이중 점프 허용(낙하 상태가 아니고, 눌렀을 때)
        if (!isFalling
            && playerController.Input.JumpPressed
            && playerController.Move.RemainingJumps > 0)
        {
            playerController.ChangeState(new PlayerJumpState(false));
            return;
        }
        if (playerController.Move.IsGrounded && Mathf.Abs(playerController.Move.Velocity.y) < 0.01f)
        {
            playerController.playerMovement.ResetJumpCount();
            playerController.Anim.Jump(2);
            playerController.ChangeState(new PlayerIdleState());
            return;

        }

        if (playerController.Move.Velocity.y < 0)
            playerController.Anim.Jump(-1);

        // 공중에서 좌우 이동
        float xInput = playerController.Input.Horizontal;
        playerController.Move.Move(xInput);
    }

    public void ExitState(PlayerController playerController)
    {
        playerController.Anim.Jump(2);
    }

}
