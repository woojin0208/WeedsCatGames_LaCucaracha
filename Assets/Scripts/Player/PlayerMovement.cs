using System.Collections;
using UnityEngine;

public class PlayerMovement : MovementBase, IDashable
{
    public int RemainingJumps => currentJumpCount;
    public bool IsClimingWall { get; private set; }
    public bool IsDashing;
    public float currentDashCooldown = 0;
    public bool IsClimbLadder { private set; get; }

    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private int currentJumpCount = 2;
    [SerializeField] private PhysicsMaterial2D[] physicMat;
    [SerializeField] private float dashCooldownTime = 1f;
    [SerializeField] private Transform footPosition;

    private float dashSpeed;
    private float jumpForce;
    private bool wasGrounded;
    private Coroutine wallClimingCoroutine;
    protected override void Awake()
    {
        base.Awake();

        currentJumpCount = maxJumpCount;

    }

    public void InitStats(float moveSpeed, float dashSpeed, float jumpForce)
    {
        base.moveSpeed = moveSpeed;
        walkSpeed = moveSpeed;
        this.dashSpeed = dashSpeed;
        this.jumpForce = jumpForce;
    }

    private void Update()
    {
        CheckGround();
        //Debug.Log(currentJumpCount);

        if (currentDashCooldown >= 0)
        {
            currentDashCooldown -= Time.deltaTime;
        }


    }
    public void Dash(float x)
    {
        if (currentDashCooldown > 0)
        {
            Debug.Log("거르기 성공");
            return;
        }
        currentDashCooldown = dashCooldownTime;
        //StopCoroutine(DashCoroutine(x));
        StartCoroutine(DashCoroutine(x));
    }

    public void OnJump()
    {
        if (currentJumpCount <= 0) return;

        if (rigidbody2D.sharedMaterial == physicMat[1])
        {
            rigidbody2D.sharedMaterial = physicMat[0];
        }
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);

        rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        if (wallClimingCoroutine != null)
        {
            StopCoroutine(wallClimingCoroutine);

            rigidbody2D.sharedMaterial = physicMat[0];
            IsClimingWall = false;
        }

        currentJumpCount--;

        int jumpCount = currentJumpCount == 1 ? 1 : 2;

    }

    public void DecrementJumpCount()
    {
        if (currentJumpCount > 0)
            currentJumpCount--;
    }

    private void CheckGround()
    {
        if (IsClimingWall)
        {
            IsGrounded = true;
            return;
        }

        float rayDistance = Mathf.Abs(rigidbody2D.position.y - footPosition.position.y);
        RaycastHit2D rayHit = Physics2D.BoxCast(
            footPosition.position,
            new Vector2(0.22f, 0.05f), // Player Collider Size.x 보다 아주 약간 작게 , 감지용 작은 y
            0,
            Vector2.down,
            0.01f,
            LayerMask.GetMask("Ground")
        );

        bool nowGrounded = rayHit.collider != null;
        IsGrounded = nowGrounded;


        if (rigidbody2D.velocity.y <= 0)
        {
            if (nowGrounded && !wasGrounded)
            {
                currentJumpCount = maxJumpCount;
            }
        }

        wasGrounded = nowGrounded;
    }

    public void ResetJumpCount() => currentJumpCount = maxJumpCount;

    public void ClimbLadder(Vector2 startPosition, Vector2 endPosition)
    {
        StartCoroutine(LadderCoroutine(startPosition, endPosition));
    }
    public IEnumerator LadderCoroutine(Vector2 startPosition, Vector2 endPosition)
    {
        IsClimbLadder = true;
        rigidbody2D.isKinematic = true;
        transform.position = startPosition;

        Vector2 moveDirection = (endPosition - startPosition).normalized;
        float sqrEpsilon = 0.0001f;

        while (Vector2.SqrMagnitude(endPosition - rigidbody2D.position) > sqrEpsilon)
        {
            rigidbody2D.velocity = moveDirection * (moveSpeed * 0.5f);
            yield return new WaitForFixedUpdate();
        }

        rigidbody2D.isKinematic = false;
        rigidbody2D.velocity = Vector2.zero;
        Debug.Log("ClimbeEnd");
        yield return null;
        IsClimbLadder = false;
    }

    private IEnumerator DashCoroutine(float x)
    {
        IsDashing = true;
        float dashDuration = 0.2f;
        float elapsedTime = 0f;
        float xDirection = rigidbody2D.velocity.x > 0 ? 1 : -1;

        while (elapsedTime < dashDuration)
        {
            OnMovement(x, true);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        IsDashing = false;
    }

    public void WallCling(float duration)
    {
        Debug.Log("WallCling 중");
        wallClimingCoroutine = StartCoroutine(WallClingCoroutine(duration));
    }
    public IEnumerator WallClingCoroutine(float duration)
    {
        IsClimingWall = true;
        rigidbody2D.sharedMaterial = physicMat[1];
        currentJumpCount = 2;
        yield return new WaitForSeconds(duration);

        rigidbody2D.sharedMaterial = physicMat[0];
        IsClimingWall = false;

    }
}
