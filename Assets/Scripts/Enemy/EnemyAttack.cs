using System.Collections;
using UnityEngine;

// 적 공격 판정과 패턴 프리팹 생성을 처리한다.
public class EnemyAttack : MonoBehaviour
{

    [SerializeField] private GameObject areaAttackPrefab;
    [SerializeField] private GameObject projectileAttackPrefab;
    [SerializeField] private float attackRangeX = 1.0f;
    [SerializeField] private float attackRangeY = 0.5f;
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

    // 애니메이션 이벤트 번호에 따라 근접, 범위, 투사체 공격을 실행한다.
    public void HandleAttackEvent(int attackNum)
    {
        if (attackNum == 0)
        {
            Vector2 dir = enemyBase.transform.localScale.x > 0 ? Vector2.left : Vector2.right;
            Vector2 origin = (Vector2)transform.position + dir * (attackRangeX * 0.5f);

            Collider2D[] hits = Physics2D.OverlapBoxAll(
                origin,
                new Vector2(attackRangeX, attackRangeY),
                0f,
                LayerMask.GetMask(GameLayers.Player)
            );

            // 근접 공격 범위 안의 플레이어에게 피해를 준다.
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<PlayerBase>(out var player))
                    player.TakeDamage(attackDamage);
            }
        }
        else if (attackNum == 1)
        {
            StartCoroutine(AreaAttackRoutine());
        }
        else if (attackNum == 2)
        {
            StartCoroutine(ProjectileAttackRoutine());
        }
    }

    private IEnumerator AreaAttackRoutine()
    {
        Vector2 areaPoint = FindObjectOfType<PlayerBase>().transform.position;
        areaPoint.y = -2.6f;
        areaPoint.x -= 0.67f;

        var fx = Instantiate(areaAttackPrefab, areaPoint, Quaternion.identity);
        Destroy(fx, 1.1f);

        yield return null;
    }

    private IEnumerator ProjectileAttackRoutine()
    {
        var fx = Instantiate(projectileAttackPrefab, new Vector2(-1.87f, -2.202f), Quaternion.identity);
        Destroy(fx, 1f);
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