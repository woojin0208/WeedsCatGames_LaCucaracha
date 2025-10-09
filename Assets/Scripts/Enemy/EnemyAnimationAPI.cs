using System;

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
