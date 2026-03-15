using System.Collections;
using System.Linq;
using UnityEngine;

// 적 상태 전이와 탐지 로직을 제어한다.
public enum EnemyState { Idle, Patrol, Chase, Attack, Die, Blind }

// 적 상태 전이와 탐지 로직을 제어한다.
public class EnemyController : StateMachine<EnemyController>, IStatusEffectHandler
{
    [Header("EnemyAI Setting")]
    [SerializeField]
    private float detectRange = 3f;
    [SerializeField]
    private float startDetectPosition = 1;

    public float MaxChaseDistance { get; } = 5f;
    [SerializeField] private float patrolDistance = 2;
    [SerializeField] private float attackDistance = 1;
    [SerializeField] private bool hasSpecialAttack;
    [SerializeField] private Transform groundCheck;

    [SerializeField] private EnemyState[] blockedStates;

    private Transform targetPosition;

    private float originPosition;
    private float patrolPosition;
    private float currentStateTime = 0;
    private float attackSpeed = 0;

    private float currentAttackTime;
    private bool isAttack;

    private EnemyBase enemyBase;
    private EnemyMovement enemyMovement;
    private EnemyRenderer enemyRenderer;
    [field: SerializeField] public float AttackDistance = 1;
    [field: SerializeField] public float IdleTime { get; private set; } = 3;
    [field: SerializeField] public float PatrolTime { get; private set; } = 3;
    [field: SerializeField] public int AttackIndex { get; private set; }
    [field: SerializeField] public float AttackTime { get; private set; }
    public MovementAPI Move { get; private set; }
    public EnemyAnimationAPI Anim { get; private set; }

    public float CurrentAttackTime { get; set; }
    private void Awake()
    {
        var movement = GetComponent<EnemyMovement>();
        Move = new MovementAPI(movement);
        Anim = new EnemyAnimationAPI(GetComponentInChildren<EnemyRenderer>());

        enemyBase = GetComponent<EnemyBase>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyRenderer = GetComponentInChildren<EnemyRenderer>();

        enemyBase.OnDamagedAction += HandleDamaged;
    }

    private void Start()
    {
        originPosition = transform.position.x;
        patrolPosition = originPosition;
        ChangeState(new EnemyIdleState());
    }

    public void Init(float attackSpeed)
    {
        this.attackSpeed = attackSpeed;
    }

    protected override void Update()
    {
        base.Update();

        CheckState();
        Debug.Log($"{gameObject.name}, {currentState}");
    }

    private void CheckState()
    {
        if (currentState is IEnemyState state)
        {
            if (state.CanChase)
            {
                TryDetectTarget();
            }
        }
    }

    private void HandleDamaged()
    {
        if (currentState is IEnemyState state)
        {
            if (state.CanHitAnim)
            {
                ChangeState(new EnemyHitState());
            }
        }

        Anim.TakeDamaged();
    }
    private void TryDetectTarget()
    {
        Vector2 rayOrigin = transform.position;
        rayOrigin.x += Anim.IsLeft ? -startDetectPosition : startDetectPosition;

        Vector2 rayDirection = Anim.IsLeft ? Vector2.right : Vector2.left;

        RaycastHit2D rayHit = Physics2D.CircleCast(
            rayOrigin,
            detectRange,
            rayDirection,
            detectRange,
            LayerMask.GetMask("Player")
        );

        if (rayHit.collider != null)
        {
            targetPosition = rayHit.collider.transform;
            ChangeState(new EnemyChaseState(targetPosition));
        }
    }

    public bool CheckGroundAhead()
    {
        if (groundCheck == null) return true;
        Vector2 rayOrigin = transform.position;
        rayOrigin.y = groundCheck.position.y;
        Vector2 rayDirection = Anim.IsLeft ? Vector2.left : Vector2.right;
        float rayDistance = groundCheck.position.x - rayOrigin.x;

        RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, LayerMask.GetMask("Ground"));

        // 전방 레이캐스트에 땅이 감지되면 계속 이동 가능하다.
        return rayHit.collider != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (Anim == null || enemyBase == null) return;
        Vector2 rayOrigin = transform.position;
        rayOrigin.x += Anim.IsLeft ? -startDetectPosition : startDetectPosition;
        Vector2 direction = Anim.IsLeft ? Vector2.right : Vector2.left;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rayOrigin, detectRange);

        Vector2 endPoint = rayOrigin + direction * detectRange;
        Gizmos.DrawWireSphere(endPoint, detectRange);
        Gizmos.DrawLine(rayOrigin, endPoint);
    }

    public void ApplyEffect(StatusEffectData effectData)
    {
        foreach (EffectTargetKind e in effectData.target)
        {
            if (!effectData.target.Contains(EffectTargetKind.Enemy)) return;
        }

        switch (effectData.kind)
        {
            case EffectKind.Blind:
                StartCoroutine(OnBlindEffect(effectData.duration));
                break;
            case EffectKind.Damage:
                break;
            case EffectKind.Slow:
                enemyBase.slowCoroutine = StartCoroutine(enemyBase.OnSlowEffect(effectData.duration, effectData.rate));
                break;
        }
    }

    public void IgnoreEffect(StatusEffectData effectData)
    {
        foreach (EffectTargetKind e in effectData.target)
        {
            if (!effectData.target.Contains(EffectTargetKind.Enemy)) return;
        }

        switch (effectData.kind)
        {
            case EffectKind.Blind:
                break;
            case EffectKind.Damage:
                break;
            case EffectKind.Slow:
                StopCoroutine(enemyBase.slowCoroutine);
                break;
        }
    }

    private IEnumerator OnBlindEffect(float duration)
    {
        ChangeState(new EnemyBlindState());
        yield return new WaitForSeconds(duration);
        Debug.Log("Blind End");

        ChangeState(new EnemyIdleState());
    }
}