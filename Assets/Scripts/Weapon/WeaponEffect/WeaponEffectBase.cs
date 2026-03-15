using UnityEngine;

// 무기 효과의 공통 동작을 제공한다.
public class WeaponEffectBase : MonoBehaviour
{
    private EffectTargetKind target;
    public bool IsLeftThrow { get; set; }

    public virtual void Initialize(EffectTargetKind target)
    {
        this.target = target;
    }
}