using System.Linq;
using UnityEngine;

// 환경 오브젝트에 적용 가능한 상태 이상 여부를 판정한다.
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

    public void IgnoreEffect(StatusEffectData effectData)
    {
        foreach (EffectTargetKind e in effectData.target)
        {
            if (effectData.target.Contains(environmentKind)) return;
        }
    }
}