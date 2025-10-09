using UnityEngine;

public class WeaponEffectBase : MonoBehaviour
{
    private EffectTargetKind target;

    public virtual void Initialize(EffectTargetKind target)
    {
        this.target = target;
    }
}
