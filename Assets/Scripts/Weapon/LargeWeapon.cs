using UnityEngine;

// 대형 무기 동작을 정의한다.
public class LargeWeapon : WeaponBase
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

    public override void Interactive(PlayerBase player)
    {
        base.Interactive(player);
    }
}