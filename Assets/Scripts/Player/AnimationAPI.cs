using UnityEngine;

// 애니메이션 관련 공통 기능을 제공한다.
public class AnimationAPI
{
    protected AnimationBase entityRenderer;

    public void Idle() => entityRenderer.WalkAnim(0);
    public void Walk(float x)
    {
        entityRenderer.WalkAnim(Mathf.Sign(x));
    }
    public void Attack(int attackNum) => entityRenderer.AttackAnim(attackNum);

    public void TakeDamaged() => entityRenderer.TakeDamaged();

    public void Die(EntityBase entity)
    {
        entityRenderer.OnDieAction += entity.OnDied;
        entityRenderer.DieAnim();
    }
}