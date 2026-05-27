using System.Collections;
using UnityEngine;

// 이동하는 NPC의 Interface를 정의한다.
public interface IMovementNPC
{
    void OnMovement();
}

// 대화 이벤트에 의해 지정된 위치까지 이동하는 NPC를 처리한다.
public class MovementNPC : NPCDialogue, IMovementNPC
{
    private static readonly int MoveDirectionHash = Animator.StringToHash("moveDirection");

    [SerializeField] private Transform targetPoint;
    [SerializeField] private float arriveDistance = 0.1f;
    [SerializeField] private bool isExpired;

    private MovementBase movementBase;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2D;
    private Coroutine movementRoutine;

    protected override void Awake()
    {
        base.Awake();

        movementBase = GetComponent<MovementBase>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void OnMovement()
    {
        if (movementRoutine != null) return;

        if (targetPoint == null)
        {
            Debug.LogWarning("[MovementNPC] targetPoint 가 null 입니다.", this);
            return;
        }

        if (movementBase == null)
        {
            Debug.LogWarning("[MovementNPC] movementBase 가 null 입니다.", this);
            return;
        }

        movementRoutine = StartCoroutine(MovementCoroutine());
    }

    private IEnumerator MovementCoroutine()
    {
        SetMoveAnimation(true);

        while (!HasArrived())
        {
            float direction = GetMoveDirection();

            movementBase.Move(direction);
            SetFlip(direction);

            yield return new WaitForFixedUpdate();
        }

        movementBase.Idle();

        SnapToTargetX();

        SetMoveAnimation(false);
        movementRoutine = null;

        RaiseDialogueSignal();

        if (isExpired) gameObject.SetActive(false);

    }

    private bool HasArrived()
    {
        float distanceX = Mathf.Abs(targetPoint.position.x - transform.position.x);
        return distanceX <= arriveDistance;
    }

    private float GetMoveDirection()
    {
        return targetPoint.position.x > transform.position.x ? 1f : -1f;
    }

    private void SnapToTargetX()
    {
        Vector2 targetPosition = transform.position;
        targetPosition.x = targetPoint.position.x;

        if (rigidbody2D != null)
        {
            rigidbody2D.position = targetPosition;
            rigidbody2D.velocity = Vector2.zero;
            return;
        }

        transform.position = targetPosition;
    }

    private void SetMoveAnimation(bool isMove)
    {
        if (animator == null) return;

        animator.SetFloat(MoveDirectionHash, isMove ? 1f : 0f);
    }

    private void SetFlip(float direction)
    {
        if (spriteRenderer == null) return;

        spriteRenderer.flipX = direction > 0f;
    }
}