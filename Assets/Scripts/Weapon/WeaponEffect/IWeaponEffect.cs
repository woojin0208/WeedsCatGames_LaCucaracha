// 무기 파괴 효과 중 충돌 정보 기반 초기화가 필요한 효과가 구현한다.
public interface IWeaponEffect
{
    void InitializeFromWeaponHit(WeaponEffectContext context);
}