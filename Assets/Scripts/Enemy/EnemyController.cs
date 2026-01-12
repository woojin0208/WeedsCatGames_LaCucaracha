using System.Collections;
using System.Linq;
using UnityEngine;

public enum EnemyState { Idle, Patrol, Chase, Attack, Die, Blind }
public class EnemyController : StateMachine<EnemyController>, IStatusEffectHandler
{
    [Header("EnemyAI Setting")]
    [SerializeField]
    private float detectRange = 3f;         // Player 탐지 거리
    [SerializeField]
    private float startDetectPosition = 1;  // Patrol 시 연산에 필요한 변수

    public float MaxChaseDistance { get; } = 5f;   // 최대 추격 거리. (탐지 시, 탐지 거리 밖까지 추격 가능하도록)
    [SerializeField] private float patrolDistance = 2;       // Patrol 거리
    [SerializeField] private float attackDistance = 1;       // 공격 거리
    [SerializeField] private bool hasSpecialAttack;          // 특수공격 존재 여부
    [SerializeField] private Transform groundCheck;

    [SerializeField] private EnemyState[] blockedStates; // 추후 추가

    private Transform targetPosition;       // Player Position

    private float originPosition;           // 기본 위치
    private float patrolPosition;           // 탐색 위치
    private float currentStateTime = 0;     //
    private float attackSpeed = 0;

    private float currentAttackTime;
    private bool isAttack;

    private EnemyBase enemyBase;
    private EnemyMovement enemyMovement;
    private EnemyRenderer enemyRenderer;
    [field: SerializeField] public float AttackDistance = 1;
    [field: SerializeField] public float IdleTime { get; private set; } = 3;             // Patrol 시 갖는 Idle 시간
    [field: SerializeField] public float PatrolTime { get; private set; } = 3;           // Patorl 시 갖는 Patrol 시간
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
        //Debug.Log(originPosition);
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
            if (state.CanChase) // 추격 가능한 상태
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
            rayOrigin,              // 감지 "시작점"
            detectRange,            // 감지 "반지름"
            rayDirection,           // 감지 "방향"
            detectRange,            // 감지 "거리"
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

        return rayHit.collider != null; // 땅이 있다면 true. 없다면 false;
    }

    private void OnDrawGizmosSelected()
    {
        if (Anim == null || enemyBase == null) return;
        Vector2 rayOrigin = transform.position;
        rayOrigin.x += Anim.IsLeft ? -startDetectPosition : startDetectPosition;
        Vector2 direction = Anim.IsLeft ? Vector2.right : Vector2.left;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rayOrigin, detectRange); // 원 시작 지점

        Vector2 endPoint = rayOrigin + direction * detectRange;
        Gizmos.DrawWireSphere(endPoint, detectRange); // 원 끝 지점
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
                //StartCoroutine(OnBlindEffect(effectData.duration));
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
