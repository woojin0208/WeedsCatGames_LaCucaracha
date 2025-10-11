using UnityEngine;

public interface IStatusEffectHandler
{
    void ApplyEffect(StatusEffectData effectData);

    void IgnoreEffect(StatusEffectData effectData);
}
