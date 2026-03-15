using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// 이동하는 NPC의 Interface를 정의한다.
public interface IMovementNPC
{
    void OnMovement();
}

// 이동하는 NPC 동작을 처리한다.
public class MovementNPC : NPCDialogue, IMovementNPC
{
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private bool isExpired;
    private MovementBase movementBase;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    protected override void Awake()
    {
        base.Awake();
        movementBase = GetComponent<MovementBase>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnMovement()
    {
        StartCoroutine(MovementCoroutine());
    }

    private IEnumerator MovementCoroutine()
    {
        while (Vector2.Distance(targetPosition, transform.position) > 0.1f)
        {
            float x = targetPosition.x - transform.position.x > 0 ? 1 : -1;

            movementBase.OnMovement(x);
            animator.SetFloat("moveDirection", 1);
            spriteRenderer.flipX = x > 0 ? true : false;
            yield return null;
        }

        movementBase.OnMovement(0);

        RaiseDialogueSignal();

        if (isExpired) gameObject.SetActive(false);

        animator.SetFloat("moveDirection", 0);
        yield return null;
    }
}