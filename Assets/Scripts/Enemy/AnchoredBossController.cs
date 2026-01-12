using System.Collections;
using UnityEngine;

public class AnchoredBossController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private AnchoredBossRenderer bossRenderer;   // Head 메인 + Body 동기화
    [SerializeField] private EnemyAttack enemyAttack;             // 지금 쓰시던 Attack 단일 클래스
    [SerializeField] private Transform player;                    // 캐싱

    [Header("Attack Loop")]
    [SerializeField] private float openDelay = 0.5f;              // 전투 시작 전 연출 대기
    [SerializeField] private float attackInterval = 1.2f;         // 패턴 간 간격
    [SerializeField] private bool loop = true;
    [SerializeField] private EnemyAttackType[] attackTypes;

    [SerializeField] private GameObject openAction;
    private bool isDead;
    private bool isBlinded;

    private void Awake()
    {
        if (player == null) player = FindObjectOfType<PlayerBase>()?.transform;
        // EnemyBase OnDamagedAction → bossRenderer.TakeDamaged() 쪽은 기존처럼 연결
    }

    public void StartBattle() => StartCoroutine(BossLoop());
    private IEnumerator BossLoop()
    {
        yield return new WaitForSeconds(openDelay);
        openAction.SetActive(true);
        yield return new WaitForSeconds(attackInterval);
        while (!isDead && loop)
        {
            if (attackTypes[0] == EnemyAttackType.MeleeAttack)
            {
                bossRenderer.AttackAnim(1);
                enemyAttack.HandleAttackEvent(0);
            }
            else
            {
                int type = Random.Range(0, attackTypes.Length); // ← Length + 1 쓰면 OOR

                if (attackTypes[type] == EnemyAttackType.AreaAttack)
                {
                    bossRenderer.AttackAnim(1);
                    enemyAttack.HandleAttackEvent(1);
                }
                else if (attackTypes[type] == EnemyAttackType.ProjectileAttack)
                {
                    bossRenderer.AttackAnim(2);
                    enemyAttack.HandleAttackEvent(2);
                }

            }

            yield return new WaitForSeconds(attackInterval);
        }
    }
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        bossRenderer.DieAnim();
        // 드랍/연출 등
    }
}
