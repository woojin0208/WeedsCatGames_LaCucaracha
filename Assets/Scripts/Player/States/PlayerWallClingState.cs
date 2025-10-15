using UnityEngine;

public class PlayerWallClingState : IPlayerState
{
    public bool CanAttack { get; } = false;
    public bool CanJump { get; } = true;
    public bool CanDash { get; } = false;

    public bool CanWalk { get; } = false;
    public bool CanLadder { get; } = false;
    public bool CanClingWall { get; } = false;
    public bool CanPipeWarp { get; } = false;

    private float duration;
    private float xDir;
    public PlayerWallClingState(float duration, float effectXPos)
    {
        this.duration = duration;
        this.xDir = effectXPos;
    }
    public void EnterState(PlayerController playerController)
    {
        playerController.playerMovement.WallCling(duration);
        playerController.Move.ChangeGravity(0.05f);

        playerController.Anim.WallCling(true, xDir);

        Debug.Log(playerController.GetComponent<Rigidbody2D>().gravityScale);
        // ¾ÆÁ÷ ¾È³ª¿È playerController.Anim.WallCling(true);
    }

    public void UpdateState(PlayerController playerController)
    {
        duration -= Time.deltaTime;

        if (duration <= 0) playerController.ChangeState(new PlayerIdleState());

        playerController.Move.Move(playerController.Input.Horizontal);
    }

    public void ExitState(PlayerController playerController)
    {
        playerController.Move.ChangeGravity(1f);

        playerController.Anim.WallCling(false, 0);
    }
}
