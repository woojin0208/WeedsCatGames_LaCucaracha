using System.Collections;
using UnityEngine;

// 적에게 사용된 상태이상 지속 상태를 관리한다.
// 행동 FSM과 분리해 Hit, Attack 같은 상태 전환 중에도 상태이상 효과가 유지되도록 한다.
public class EnemyStatusEffectController : MonoBehaviour
{
    private EnemyMovement enemyMovement;
    private EnemyBase enemyBase;
    private EnemyAttack enemyAttack;

    private Coroutine blindCoroutine;
    private Coroutine slowCoroutine;
    private Coroutine stunCoroutine;
    private Coroutine attackDownCoroutine;

    private Stat moveSpeedStat;
    private Stat attackDamageStat;

    private float currentSlowBonus;
    private float currentAttackDownBonus;

    public bool IsBlinded { get; private set; }
    public bool IsStunned { get; private set; }

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyBase = GetComponent<EnemyBase>();
        enemyAttack = GetComponentInChildren<EnemyAttack>();

        if (enemyBase != null && enemyBase.stats != null)
        {
            moveSpeedStat = enemyBase.stats.GetStat(StatType.MoveSpeed);
            attackDamageStat = enemyBase.stats.GetStat(StatType.AttackDamage);
        }
    }
    public void ApplyBlind(float duration)
    {
        if (blindCoroutine != null)
        {
            StopCoroutine(blindCoroutine);
        }

        blindCoroutine = StartCoroutine(BlindRoutine(duration));
    }

    public void ApplySlow(float duration, float rate)
    {
        RemoveSlow();

        if (moveSpeedStat == null) return;

        slowCoroutine = StartCoroutine(SlowRoutine(duration, rate));
    }

    public void ApplyStun(float duration)
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        stunCoroutine = StartCoroutine(StunRoutine(duration));
    }

    public void ApplyAttackDown(float duration, float rate)
    {
        RemoveAttackDown();

        if (attackDamageStat == null) return;

        attackDownCoroutine = StartCoroutine(AttackDownRoutine(duration, rate));
    }

    public void RemoveSlow()
    {
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            slowCoroutine = null;
        }

        if (moveSpeedStat == null) return;
        if (Mathf.Approximately(currentSlowBonus, 0f)) return;

        moveSpeedStat.BonusValue -= currentSlowBonus;
        enemyMovement?.SetMoveSpeed(moveSpeedStat.Value);
        currentSlowBonus = 0f;
    }

    public void RemoveAttackDown()
    {
        if (attackDownCoroutine != null)
        {
            StopCoroutine(attackDownCoroutine);
            attackDownCoroutine = null;
        }

        if (attackDamageStat == null) return;
        if (Mathf.Approximately(currentAttackDownBonus, 0f)) return;

        attackDamageStat.BonusValue -= currentAttackDownBonus;
        enemyAttack?.Init(attackDamageStat.Value);
        currentAttackDownBonus = 0f;
    }

    private IEnumerator BlindRoutine(float duration)
    {
        IsBlinded = true;

        yield return new WaitForSeconds(duration);

        IsBlinded = false;
        blindCoroutine = null;
    }

    private IEnumerator SlowRoutine(float duration, float rate)
    {
        currentSlowBonus = -(moveSpeedStat.DefaultValue * (rate / 100f));
        moveSpeedStat.BonusValue += currentSlowBonus;
        enemyMovement?.SetMoveSpeed(moveSpeedStat.Value);

        yield return new WaitForSeconds(duration);

        RemoveSlow();
    }

    private IEnumerator StunRoutine(float duration)
    {
        IsStunned = true;
        enemyMovement?.SetMoveSpeed(0);

        yield return new WaitForSeconds(duration);

        IsStunned = false;
        enemyMovement?.SetMoveSpeed(moveSpeedStat != null ? moveSpeedStat.Value : 0f);
        stunCoroutine = null;
    }

    private IEnumerator AttackDownRoutine(float duration, float rate)
    {
        currentAttackDownBonus = -(attackDamageStat.DefaultValue * (rate / 100f));
        attackDamageStat.BonusValue += currentAttackDownBonus;
        enemyAttack?.Init(attackDamageStat.Value);

        yield return new WaitForSeconds(duration);

        RemoveAttackDown();
    }

    private void OnDisable()
    {
        if (blindCoroutine != null)
        {
            StopCoroutine(blindCoroutine);
            blindCoroutine = null;
        }

        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
            stunCoroutine = null;
        }

        IsBlinded = false;
        IsStunned = false;

        RemoveSlow();
        RemoveAttackDown();
    }
}