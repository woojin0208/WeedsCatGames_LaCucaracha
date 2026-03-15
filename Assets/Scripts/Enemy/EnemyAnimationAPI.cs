using System;

// 적 애니메이션 이벤트 접근을 단순화한다.
public class EnemyAnimationAPI : AnimationAPI
{
    private readonly EnemyRenderer enemyRenderer;

    public bool EndAttack { get; set; } = false;
    public EnemyAnimationAPI(EnemyRenderer enemyRenderer)
    {
        entityRenderer = enemyRenderer;
        this.enemyRenderer = enemyRenderer;
    }

    public event Action EndHitEvent
    {
        add => enemyRenderer.EndHitEvent += value;
        remove => enemyRenderer.EndHitEvent -= value;
    }

    public event Action<int> OnAttackEvent
    {
        add => entityRenderer.OnAttackEvent += value;
        remove => entityRenderer.OnAttackEvent -= value;
    }

    public void EndAttackAnim(int onAttack) => EndAttack = onAttack == 0 ? true : false;

    public void HitAnim()
    {
        enemyRenderer.TakeDamage();
    }

    public bool IsLeft => enemyRenderer.IsLeft;
}