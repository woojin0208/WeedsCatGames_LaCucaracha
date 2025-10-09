using UnityEngine;

public class PlayerWallClingState : IPlayerState
{
    public bool CanAttack { get; } = false;
    public bool CanJump { get; } = true;
    public bool CanDash { get; } = false;

    public bool CanWalk { get; } = true;
    public bool CanLadder { get; } = false;
    public bool CanClingWall { get; } = false;
    private float duration;
    public PlayerWallClingState(float duration)
    {
        this.duration = duration;
    }
    public void EnterState(PlayerController playerController)
    {
        Debug.Log(playerController.name);
        playerController.playerMovement.WallCling(duration);
        
        // ¾ÆÁ÷ ¾È³ª¿È playerController.Anim.WallCling(true);
    }

    public void UpdateState(PlayerController playerController)
    {
        Debug.Log(playerController.Move.Velocity);
        playerController.Anim.Idle();
        if (!playerController.playerMovement.IsClimingWall) playerController.ChangeState(new PlayerIdleState());
    }

    public void ExitState(PlayerController playerController)
    {
        //playerController.playerMovement.WallCling(duration, false);
    }
}
