using UnityEngine;

// 이동 컴포넌트가 구현해야 하는 공통 동작을 정의한다.
public interface IMovement
{
    Vector2 CurrentVelocity { get; }
    float HorizontalDirection { get; }
    bool IsGrounded { get; set; }
    void Move(float x);
    void Idle();
    void ChangeGravity(float scale);
    void Stop(bool isStop);
}

// 대시 기능이 있는 이동 컴포넌트 규약을 정의한다.
public interface IDashable
{
    void Dash(float x);
}

// 리지드바디 기반 이동의 공통 처리를 제공한다.
public abstract class MovementBase : MonoBehaviour, IMovement
{
    [SerializeField] protected float moveSpeed;
    protected float walkSpeed;
    protected Rigidbody2D rigidbody2D;

    protected virtual void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public Vector2 CurrentVelocity => rigidbody2D.velocity;
    public float HorizontalDirection => Mathf.Sign(rigidbody2D.velocity.x);
    public bool IsGrounded { get; set; }

    public void Stop(bool isStop)
    {
        rigidbody2D.gravityScale = isStop ? 0f : 1f;
        rigidbody2D.velocity = Vector2.zero;
    }

    public virtual void Move(float x) => OnMovement(x, false);
    public virtual void Idle() => OnMovement(0, false);

    public virtual void OnMovement(float x, bool isDash = false)
    {
        float yVelocity = isDash ? 0f : rigidbody2D.velocity.y;

        if (isDash)
        {
            Vector2 moveDirection = new Vector2(x * moveSpeed * 4, yVelocity);
            rigidbody2D.velocity = moveDirection;
        }
        else
        {
            Vector2 moveDirection = new Vector2(x * moveSpeed, yVelocity);
            rigidbody2D.velocity = moveDirection;
        }
    }

    public void ChangeGravity(float scale)
    {
        rigidbody2D.velocity = Vector2.zero;

        rigidbody2D.gravityScale = scale;
    }
}