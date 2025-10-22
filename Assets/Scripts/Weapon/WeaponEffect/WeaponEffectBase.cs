using UnityEngine;

public class WeaponEffectBase : MonoBehaviour
{
    private EffectTargetKind target;
    public bool IsLeftThrow { get; set; }

    public virtual void Initialize(EffectTargetKind target)
    {
        this.target = target;
    }
}
