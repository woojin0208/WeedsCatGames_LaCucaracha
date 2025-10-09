using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackRangeX = 1.0f;
    [SerializeField] private float attackRangeY = 0.5f;

    private EnemyRenderer enemyRenderer;
    private EnemyBase enemyBase;
    
    private float attackDamage;
    private void Awake()
    {
        enemyBase = GetComponentInParent<EnemyBase>();
        enemyRenderer = transform.parent.GetComponentInChildren<EnemyRenderer>();
    }

    private void Start()
    {
        enemyRenderer.OnAttackEvent += HandleAttackEvent;
    }

    public void Init(float attackDamage)
    {
        this.attackDamage = attackDamage;
    }

    /// <summary>
    /// 1 : AttackTiming / 2 : EndAttackTiming / 2 : EndAttack
    /// </summary>
    /// <param name="isAttackTiming"></param>
    private void HandleAttackEvent(int isAttackTiming)
    {
        if (isAttackTiming == 1)
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
