using System.Collections;
using UnityEngine;

// 적 공격 판정과 패턴 프리팹 생성을 처리한다.
public class EnemyAttack : MonoBehaviour
{
    private const int MeleeAttackEventIndex = 0;
    private const int AreaAttackEventIndex = 1;
    private const int ProjectileAttackEventIndex = 2;

    [SerializeField] private GameObject areaAttackPrefab;
    [SerializeField] private GameObject projectileAttackPrefab;

    [Header("Melee Attack")]
    [SerializeField] private float attackRangeX = 1.0f;
    [SerializeField] private float attackRangeY = 0.5f;

    [Header("Area Attack")]
    [SerializeField] private Vector2 areaAttackOffset = new Vector2(-0.67f, 0f);
    [SerializeField] private float areaAttackYPosition = -2.6f;
    [SerializeField] private float areaAttackLifeTime = 1.1f;

    [Header("Projectile Attack")]
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Vector2 projectileSpawnPosition = new Vector2(-1.87f, -2.202f);
    [SerializeField] private float projectileLifeTime = 1f;

    private EnemyBase enemyBase;
    private float attackDamage = 10;

    private void Awake()
    {
        enemyBase = GetComponentInParent<EnemyBase>();
    }

    public void Init(float attackDamage)
    {
        this.attackDamage = attackDamage;
    }

    private Transform GetPlayerTransform()
    {
        return PlayerManager.Instance != null
            ? PlayerManager.Instance.PlayerTransform
            : null;
    }

    // 애니메이션 이벤트 번호에 따라 근접, 범위, 투사체 공격을 실행한다.
    public void HandleAttackEvent(int attackNum)
    {
        switch (attackNum)
        {
            case MeleeAttackEventIndex:
                ExecuteMeleeAttack();
                break;
            case AreaAttackEventIndex:
                StartCoroutine(AreaAttackRoutine());
                break;
            case ProjectileAttackEventIndex:
                StartCoroutine(ProjectileAttackRoutine());
                break;
        }
    }

    private void ExecuteMeleeAttack()
    {
        if (enemyBase == null) return;

        Vector2 dir = enemyBase.transform.localScale.x > 0 ? Vector2.left : Vector2.right;
        Vector2 origin = (Vector2)transform.position + dir * (attackRangeX * 0.5f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            origin,
            new Vector2(attackRangeX, attackRangeY),
            0f,
            LayerMask.GetMask(GameLayers.Player)
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<PlayerBase>(out PlayerBase target))
                target.TakeDamage(attackDamage);
        }
    }

    private IEnumerator AreaAttackRoutine()
    {
        if (areaAttackPrefab == null) yield break;

        Transform target = GetPlayerTransform();
        if (target == null) yield break;

        Vector2 areaPoint = target.position;
        areaPoint.y = areaAttackYPosition;
        areaPoint += areaAttackOffset;

        var fx = Instantiate(areaAttackPrefab, areaPoint, Quaternion.identity);
        Destroy(fx, areaAttackLifeTime);

        yield return null;
    }

    private IEnumerator ProjectileAttackRoutine()
    {
        if (projectileAttackPrefab == null) yield break;

        Vector2 spawnPosition = projectileSpawnPoint != null
            ? projectileSpawnPoint.position
            : projectileSpawnPosition;

        var fx = Instantiate(projectileAttackPrefab, spawnPosition, Quaternion.identity);
        Destroy(fx, projectileLifeTime);
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(GameTags.Player)) return;
        if (!collision.TryGetComponent<PlayerBase>(out PlayerBase player)) return;

        player.TakeDamage(attackDamage);
    }
}

// 적 공격 패턴 종류를 정의한다.
public enum EnemyAttackType { MeleeAttack, ProjectileAttack, AreaAttack, GlobalAttack }