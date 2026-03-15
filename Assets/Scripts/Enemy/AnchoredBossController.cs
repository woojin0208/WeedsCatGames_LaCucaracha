using System.Collections;
using UnityEngine;

// 고정형 보스의 공격 루프를 제어한다.
public class AnchoredBossController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private AnchoredBossRenderer bossRenderer;
    [SerializeField] private EnemyAttack enemyAttack;
    [SerializeField] private Transform player;

    [Header("Attack Loop")]
    [SerializeField] private float openDelay = 0.5f;
    [SerializeField] private float attackInterval = 1.2f;
    [SerializeField] private bool loop = true;
    [SerializeField] private EnemyAttackType[] attackTypes;

    [SerializeField] private GameObject openAction;
    private bool isDead;
    private bool isBlinded;

    private void Awake()
    {
        if (player == null) player = FindObjectOfType<PlayerBase>()?.transform;
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
                // 공격 배열 길이 범위 안에서만 랜덤 인덱스를 선택한다.
                int type = Random.Range(0, attackTypes.Length);

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
    }
}