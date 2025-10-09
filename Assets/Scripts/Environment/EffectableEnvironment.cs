using System.Linq;
using UnityEngine;

public class EffectableEnvironment : MonoBehaviour, IStatusEffectHandler
{
    [SerializeField]
    private EffectTargetKind environmentKind;
    public void ApplyEffect(StatusEffectData effectData)
    {
        foreach (EffectTargetKind e in effectData.target)
        {
            if (effectData.target.Contains(environmentKind)) return;
        }
    }

}
