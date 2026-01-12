using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private EnemyAttackType[] attackTypes;

    [SerializeField] private GameObject areaAttackPrefab;
    [SerializeField] private GameObject projectileAttackPrefab;
    [SerializeField] private float attackRangeX = 1.0f;
    [SerializeField] private float attackRangeY = 0.5f;

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Vector2 boxSize = new Vector2(3.736842f, 1.6f);
    [SerializeField] private Vector2 boxOffset = new Vector2(0f, -0.69f);

    private EnemyRenderer enemyRenderer;
    private EnemyBase enemyBase;

    private float attackDamage = 10;

    private void Awake()
    {
        enemyBase = GetComponentInParent<EnemyBase>();
        enemyRenderer = transform.parent.GetComponentInChildren<EnemyRenderer>();
    }

    private void Start()
    {

    }

    public void Init(float attackDamage)
    {
        this.attackDamage = attackDamage;
    }

    /// <summary>
    /// 1 : AttackTiming / 2 : EndAttackTiming / 2 : EndAttack
    /// </summary>
    /// <param name="isAttackTiming"></param>
    public void HandleAttackEvent(int attackNum)
    {
        if (attackNum == 0)
        {
            Vector2 dir = enemyBase.transform.localScale.x > 0 ? Vector2.left : Vector2.right;
            Vector2 origin = (Vector2)transform.position + dir * (attackRangeX * 0.5f);

            // 2) OverlapBoxAll 호출 (angle = 0)
            Collider2D[] hits = Physics2D.OverlapBoxAll(origin, new Vector2(attackRangeX, attackRangeY), 0f, LayerMask.GetMask("Player"));

            // 3) 적에게 데미지
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



    /*
    private void OnDrawGizmosSelected()
    {
        // 씬 뷰에 항상 보이게 하려면 이걸로 대체 가능
        Gizmos.color = Color.red;

        Vector2 dir = enemyBase.transform.localScale.x < 0 ? Vector2.left : Vector2.right;

        Vector2 origin = (Vector2)transform.position + dir * (attackRangeX * 0.5f);

        Gizmos.DrawWireCube(origin, new Vector2(attackRangeX, attackRangeY));
    }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        float totalDamage = attackDamage;
        if (collision.CompareTag("Player"))
        {
            Debug.Log("attack to player");
            collision.TryGetComponent<PlayerBase>(out var player);

            player.TakeDamage(totalDamage);
        }
    }

}

public enum EnemyAttackType { MeleeAttack, ProjectileAttack, AreaAttack, GlobalAttack }
