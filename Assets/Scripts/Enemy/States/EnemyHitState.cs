public class EnemyHitState : IEnemyState
{
    public bool CanHitAnim { get; } = false;
    public bool CanChase { get; } = false;

    private EnemyController controller;
    public void EnterState(EnemyController enemyController)
    {
        controller = enemyController;
        enemyController.Anim.EndHitEvent += HandleEndHitAnim;
        enemyController.Anim.HitAnim();
    }

    public void UpdateState(EnemyController enemyController)
    {
        enemyController.Move.Idle(0);
    }

    public void ExitState(EnemyController enemyController)
    {
        enemyController.Anim.EndHitEvent -= HandleEndHitAnim;
    }

    private void HandleEndHitAnim()
    {
        controller.Anim.EndHitEvent -= HandleEndHitAnim;
        controller.ChangeState(new EnemyIdleState());
    }
}
