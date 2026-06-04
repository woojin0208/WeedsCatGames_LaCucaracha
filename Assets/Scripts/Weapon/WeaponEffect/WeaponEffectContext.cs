using UnityEngine;

// 무기 파괴 효과가 충돌 상황을 기준으로 초기화될 때 필요한 정보를 전달한다.
public readonly struct WeaponEffectContext
{
    public EffectTargetKind Target { get; }
    public Vector2 ThrowDirection { get; }
    public Vector3 HitPosition { get; }

    public WeaponEffectContext(EffectTargetKind target, Vector2 throwDirection, Vector3 hitPosition)
    {
        Target = target;
        ThrowDirection = throwDirection;
        HitPosition = hitPosition;
    }
}
