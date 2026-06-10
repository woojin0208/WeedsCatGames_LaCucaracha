using System.Collections;
using UnityEngine;

// 고정형 보스의 전투 시작과 공격 루프를 제어한다.
public class AnchoredBossController : MonoBehaviour
{
    private const int AreaAttackEventIndex = 1;
    private const int ProjectileAttackEventIndex = 2;

    private const int AreaAttackAnimIndex = 1;
    private const int ProjectileAttackAnimIndex = 2;

    [SerializeField] private AnchoredBossRenderer bossRenderer;
    [SerializeField] private EnemyAttack enemyAttack;

    [SerializeField] private float openDelay = 0.5f;
    [SerializeField] private float attackInterval = 1.2f;
    [SerializeField] private bool loop = true;
    [SerializeField] private EnemyAttackType[] attackTypes;

    [SerializeField] private GameObject openAction;

    private Coroutine battleCoroutine;
    private bool isDead;

    public void StartBattle()
    {
        if (battleCoroutine != null) return;
        if (!CanStartBattle()) return;

        battleCoroutine = StartCoroutine(BossLoop());
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        if (battleCoroutine != null)
        {
            StopCoroutine(battleCoroutine);
            battleCoroutine = null;
        }

        bossRenderer?.DieAnim();
    }

    private IEnumerator BossLoop()
    {
        yield return new WaitForSeconds(openDelay);

        if (openAction != null)
        {
            openAction.SetActive(true);
        }

        yield return new WaitForSeconds(attackInterval);

        while (!isDead && loop)
        {
            ExecuteRandomAttack();

            yield return new WaitForSeconds(attackInterval);
        }

        battleCoroutine = null;
    }

    private bool CanStartBattle()
    {
        if (isDead) return false;

        if (bossRenderer == null)
        {
            Debug.LogWarning("[AnchoredBossController] BossRenderer가 null 입니다.", this);
            return false;
        }

        if (enemyAttack == null)
        {
            Debug.LogWarning("[AnchoredBossController] EnemyAttack이 null 입니다.", this);
            return false;
        }

        if (attackTypes == null || attackTypes.Length == 0)
        {
            Debug.LogWarning("[AnchoredBossController] AttackTypes가 비어 있습니다.", this);
            return false;
        }

        return true;
    }

    private void ExecuteRandomAttack()
    {
        EnemyAttackType attackType = GetRandomAttackType();

        switch (attackType)
        {
            case EnemyAttackType.AreaAttack:
                ExecuteAttack(AreaAttackAnimIndex, AreaAttackEventIndex);
                break;

            case EnemyAttackType.ProjectileAttack:
                ExecuteAttack(ProjectileAttackAnimIndex, ProjectileAttackEventIndex);
                break;

            default:
                Debug.LogWarning($"[AnchoredBossController] 지원하지 않는 보스 공격 타입입니다. type: {attackType}", this);
                break;
        }
    }

    private EnemyAttackType GetRandomAttackType()
    {
        int index = Random.Range(0, attackTypes.Length);
        return attackTypes[index];
    }

    private void ExecuteAttack(int attackAnimIndex, int attackEventIndex)
    {
        bossRenderer.AttackAnim(attackAnimIndex);
        enemyAttack.HandleAttackEvent(attackEventIndex);
    }
}