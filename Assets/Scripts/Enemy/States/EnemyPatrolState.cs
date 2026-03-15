using UnityEngine;

// 적 순찰 상태를 정의한다.
public class EnemyPatrolState : IEnemyState
{
    public bool CanHitAnim { get; } = true;
    public bool CanChase { get; } = true;

    private float patrolTime;
    private float stateTime;

    private float xDirection;
    public void EnterState(EnemyController enemyController)
    {
        xDirection = enemyController.Anim.IsLeft ? 1 : -1;
        patrolTime = enemyController.PatrolTime;
        stateTime = patrolTime;
    }

    public void UpdateState(EnemyController enemyController)
    {
        stateTime -= Time.deltaTime;

        if (stateTime <= 0)
        {
            enemyController.ChangeState(new EnemyIdleState());
            return;
        }

        if (enemyController.CheckGroundAhead())
        {
            enemyController.Move.Move(xDirection);
        }
        else
        {
            Debug.Log("Ground 발견 못함.");
            enemyController.Move.Move(0);
        }
        enemyController.Anim.Walk(xDirection);
    }

    public void ExitState(EnemyController enemyController)
    {
    }
}