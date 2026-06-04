using UnityEngine;

// 적 공격 상태를 정의한다.
public class EnemyAttackState : IEnemyState
{
    public bool CanHitAnim { get; } = false;
    public bool CanChase { get; } = false;

    public void EnterState(EnemyController enemyController)
    {
        enemyController.Anim.EndAttack = false;
        enemyController.Anim.OnAttackEvent += enemyController.Anim.EndAttackAnim;

        int maxAttackIndex = Mathf.Max(1, enemyController.AttackIndex);
        int attackNum = Random.Range(1, maxAttackIndex + 1);

        enemyController.Move.Idle(0);
        enemyController.Anim.Attack(attackNum);
    }

    public void UpdateState(EnemyController enemyController)
    {
        if (enemyController.Anim.EndAttack)
        {
            enemyController.Anim.Attack(0);
            enemyController.ChangeState(new EnemyIdleState());
        }
    }

    public void ExitState(EnemyController enemyController)
    {
        enemyController.Anim.OnAttackEvent -= enemyController.Anim.EndAttackAnim;

        enemyController.Anim.Attack(0);
        enemyController.CurrentAttackTime = Time.time;
    }
}