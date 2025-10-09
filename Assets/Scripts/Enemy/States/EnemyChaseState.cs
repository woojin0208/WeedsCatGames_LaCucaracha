using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    private Transform targetPosition;
    private float chaseDirection;

    public bool CanHitAnim { get; } = true;
    public bool CanChase { get; } = false;

    public EnemyChaseState(Transform targetPosition)
    {
        this.targetPosition = targetPosition;
    }
    public void EnterState(EnemyController enemyController)
    {
        chaseDirection = targetPosition.position.x - enemyController.transform.position.x > 0 ? 1 : -1;
        enemyController.Move.Move(chaseDirection);
        enemyController.Anim.Walk(chaseDirection);
    }

    public void UpdateState(EnemyController enemyController)
    {
        chaseDirection = targetPosition.position.x - enemyController.transform.position.x > 0 ? 1 : -1;
        enemyController.Anim.Walk(chaseDirection);
        if (!enemyController.CheckGroundAhead())
        {
            enemyController.Move.Move(0);
            enemyController.Anim.Idle();
        }
        else
        {
            //Debug.Log(chaseDirection);
            enemyController.Move.Move(chaseDirection);
            enemyController.Anim.Walk(chaseDirection);
        }

        float distance = Vector2.Distance(enemyController.transform.position, targetPosition.position);
        if (distance >= enemyController.MaxChaseDistance)
        {
            enemyController.ChangeState(new EnemyIdleState());
        }
        else if (distance <= enemyController.AttackDistance)
        {
            if (Time.time >= enemyController.CurrentAttackTime + enemyController.AttackTime)
                enemyController.ChangeState(new EnemyAttackState());
            else enemyController.Move.Idle(0);
        }
    }

    public void ExitState(EnemyController enemyContoller)
    {

    }
}
