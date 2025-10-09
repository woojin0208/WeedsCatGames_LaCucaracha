using UnityEngine;

public class SmallWeapon : WeaponBase
{
    public override void GetWeapon()
    {
        base.GetWeapon();
    }

    public override void PutWeapon()
    {
        base.PutWeapon();
    }

    public override void OnAttack()
    {
        base.OnAttack();
    }

    public override void OnThrow(Vector2 throwPosition)
    {
        base.OnThrow(throwPosition);
    }

    public override void Interactive(EntityBase entity)
    {
        base.Interactive(entity);

    }
}
