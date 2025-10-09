using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    public bool CanHitAnim { get; } = true;
    public bool CanChase { get; } = true;

    private float stateTime;
    private float idleTime;
    public void EnterState(EnemyController enemyController)
    {
        idleTime = enemyController.IdleTime;
        stateTime = idleTime;

        enemyController.Move.Idle(0);
        enemyController.Anim.Idle();

    }

    public void UpdateState(EnemyController enemyController)
    {
        stateTime -= Time.deltaTime;

        if (stateTime <= 0)
        {
            enemyController.ChangeState(new EnemyPatrolState());
            return;
        }

    }

    public void ExitState(EnemyController enemyController)
    {

    }
}
