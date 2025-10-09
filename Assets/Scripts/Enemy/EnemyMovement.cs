public class EnemyMovement : MovementBase
{
    public bool IsStop { get; private set; }

    public void Init(float moveSpeed)
    {
        base.moveSpeed = moveSpeed;
    }
}
