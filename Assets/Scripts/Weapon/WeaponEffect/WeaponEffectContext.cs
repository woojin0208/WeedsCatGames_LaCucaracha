using UnityEngine;

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
