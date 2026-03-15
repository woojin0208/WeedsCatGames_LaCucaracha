// 적 이동 속도 초기화를 담당한다.
public class EnemyMovement : MovementBase
{
    public bool IsStop { get; private set; }

    public void Init(float moveSpeed)
    {
        base.moveSpeed = moveSpeed;
    }
}