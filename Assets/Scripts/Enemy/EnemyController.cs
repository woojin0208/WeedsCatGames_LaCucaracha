using System.Linq;
using UnityEngine;

// 적 상태 전이와 탐지 로직을 제어한다.
public enum EnemyState { Idle, Patrol, Chase, Attack, Die, Blind }

// 적 상태 전이와 탐지 로직을 제어한다.
public class EnemyController : StateMachine<EnemyController>, IStatusEffectHandler
{
    [Header("EnemyAI Setting")]
    [SerializeField] private float detectRange = 3f;
    [SerializeField] private float startDetectPosition = 1;
    [SerializeField] private Transform groundCheck;

    public float MaxChaseDistance { get; } = 5f;

    [field: SerializeField] public float AttackDistance = 1;
    [field: SerializeField] public float IdleTime { get; private set; } = 3;
    [field: SerializeField] public float PatrolTime { get; private set; } = 3;
    [field: SerializeField] public int AttackIndex { get; private set; }
    [field: SerializeField] public float AttackTime { get; private set; }

    public MovementAPI Move { get; private set; }
    public EnemyAnimationAPI Anim { get; private set; }
    public float CurrentAttackTime { get; set; }

    private Transform targetPosition;
    private float attackSpeed = 0;
    private EnemyBase enemyBase;
    private EnemyStatusEffectController statusEffects;

    private void Awake()
    {
        EnemyMovement movement = GetComponent<EnemyMovement>();
        EnemyRenderer renderer = GetComponentInChildren<EnemyRenderer>();

        Move = new MovementAPI(movement);
        Anim = new EnemyAnimationAPI(renderer);

        enemyBase = GetComponent<EnemyBase>();
        statusEffects = GetComponent<EnemyStatusEffectController>();

        if (enemyBase != null)
        {
            enemyBase.OnDamagedAction += HandleDamaged;
        }
    }

    private void Start()
    {
        ChangeState(new EnemyIdleState());
    }

    private void OnDisable()
    {
        if (enemyBase != null) enemyBase.OnDamagedAction -= HandleDamaged;
    }

    public void Init(float attackSpeed)
    {
        this.attackSpeed = attackSpeed;
    }

    protected override void Update()
    {
        if (statusEffects != null && statusEffects.IsStunned)
        {
            Move.Idle(0);
            return;
        }

        base.Update();

        CheckState();
    }

    private void CheckState()
    {
        if (statusEffects != null && (statusEffects.IsBlinded || statusEffects.IsStunned)) return;

        if (currentState is not IEnemyState state) return;
        if (!state.CanChase) return;

        TryDetectTarget();
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
            LayerMask.GetMask(GameLayers.Player)
        );

        if (rayHit.collider == null) return;

        targetPosition = rayHit.collider.transform;
        ChangeState(new EnemyChaseState(targetPosition));

    }

    public bool CheckGroundAhead()
    {
        if (groundCheck == null) return true;

        Vector2 rayOrigin = transform.position;
        rayOrigin.y = groundCheck.position.y;

        Vector2 rayDirection = Anim.IsLeft ? Vector2.left : Vector2.right;
        float rayDistance = Mathf.Abs(groundCheck.position.x - rayOrigin.x);

        RaycastHit2D rayHit = Physics2D.Raycast(
            rayOrigin,
            rayDirection,
            rayDistance,
            LayerMask.GetMask(GameLayers.Ground)
        );

        return rayHit.collider != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (Anim == null) return;

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
        if (!effectData.target.Contains(EffectTargetKind.Enemy)) return;

        switch (effectData.kind)
        {
            case EffectKind.Blind:
                statusEffects?.ApplyBlind(effectData.duration);
                break;
            case EffectKind.Damage:
                break;
            case EffectKind.Slow:
                statusEffects?.ApplySlow(effectData.duration, effectData.rate);
                break;
            case EffectKind.Stun:
                statusEffects?.ApplyStun(effectData.duration);
                break;
            case EffectKind.AttackDown:
                statusEffects?.ApplyAttackDown(effectData.duration, effectData.rate);
                break;
        }
    }

    public void IgnoreEffect(StatusEffectData effectData)
    {
        if (!effectData.target.Contains(EffectTargetKind.Enemy)) return;

        switch (effectData.kind)
        {
            case EffectKind.Blind:
                break;
            case EffectKind.Damage:
                break;
            case EffectKind.Slow:
                statusEffects?.RemoveSlow();
                break;
            case EffectKind.AttackDown:
                statusEffects?.RemoveAttackDown();
                break;
        }
    }
}
