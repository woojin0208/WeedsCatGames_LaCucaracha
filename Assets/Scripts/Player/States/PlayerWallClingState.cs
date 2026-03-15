using UnityEngine;

// PlayerWallClingState 상태를 정의한다.
public class PlayerWallClingState : IPlayerState
{
    public bool CanAttack { get; } = false;
    public bool CanJump { get; } = true;
    public bool CanDash { get; } = false;
    public bool CanWalk { get; } = false;
    public bool CanLadder { get; } = false;
    public bool CanClingWall { get; } = true;
    public bool CanPipeWarp { get; } = false;

    private StatusEffectData effectData;
    private float duration;
    private float xDir;

    public PlayerWallClingState(StatusEffectData effectData)
    {
        this.effectData = effectData;
        this.duration = effectData.duration;
        this.xDir = effectData.xDir;
    }

    public void EnterState(PlayerController playerController)
    {
        playerController.Move.ChangeGravity(0.05f);
        playerController.Move.ResetJump();
        playerController.Anim.WallCling(true, xDir);
    }

    public void UpdateState(PlayerController playerController)
    {
        duration -= Time.deltaTime;
        playerController.Anim.WallCling(true, xDir);
        if (duration <= 0) playerController.ChangeState(new PlayerIdleState());
    }

    public void ExitState(PlayerController playerController)
    {
        playerController.Move.ChangeGravity(1f);
        playerController.Anim.WallCling(false, 0);
    }
}