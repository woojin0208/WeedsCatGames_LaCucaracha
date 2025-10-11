using UnityEngine;

public class MovementAPI
{
    private readonly IMovement _movement;
    private readonly IDashable _dashable;

    public MovementAPI(IMovement movement, IDashable dashable = null)
    {
        _movement = movement;
        _dashable = dashable;
    }

    // Commands
    public void Idle(float x) => _movement.Move(x);

    public void Move(float x) => _movement.Move(x);

    public void Dash(float x) => _dashable?.Dash(x);

    public void ChangeGravity(float gravity) => _movement.ChangeGravity(gravity);
    
    //public void Jump() => _movement.Jump();
    //public void ClimbLadder => 
    //public void WallCling(
    // State
    public Vector2 Velocity => _movement.CurrentVelocity;
    public float HorizontalDirection => _movement.HorizontalDirection;

    public bool IsGrounded => _movement.IsGrounded;

    public int RemainingJumps => (_movement is PlayerMovement pm) ? pm.RemainingJumps : 2;

    public bool IsDashing => (_movement is PlayerMovement pm) ? pm.IsDashing : false;
}
