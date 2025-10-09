using UnityEngine;

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
