using UnityEngine;

// 이동 관련 공통 기능을 제공한다.
public class MovementAPI
{
    private readonly IMovement _movement;
    private readonly IDashable _dashable;

    public MovementAPI(IMovement movement, IDashable dashable = null)
    {
        _movement = movement;
        _dashable = dashable;
    }

    public void Stop(bool isStop) => _movement.Stop(isStop);
    public void Idle(float x) => _movement.Move(x);

    public void Move(float x) => _movement.Move(x);

    public void Dash(float x) => _dashable?.Dash(x);

    public void ChangeGravity(float gravity) => _movement.ChangeGravity(gravity);

    public void ResetJump()
    {
        if (_movement is PlayerMovement pm) pm.ResetJumpCount();
    }

    public void Jump()
    {
        if (_movement is PlayerMovement pm) pm.OnJump();
    }
    public Vector2 Velocity => _movement.CurrentVelocity;
    public float HorizontalDirection => _movement.HorizontalDirection;

    public bool IsGrounded => _movement.IsGrounded;

    public int RemainingJumps => (_movement is PlayerMovement pm) ? pm.RemainingJumps : 2;

    public bool IsDashing => (_movement is PlayerMovement pm) ? pm.IsDashing : false;
}