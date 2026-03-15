using UnityEngine;

// 효과를 적용할 수 있는 무기 동작을 처리한다.
public class EffectableWeapon : MonoBehaviour
{
    [SerializeField]
    private GameObject effectPrefab;

    private bool isInstantiated = false;

    public void OnDestruction(EffectTargetKind target, bool isLeftThrow)
    {
        if (isInstantiated) return;

        GameObject effect = Instantiate(effectPrefab);
        effect.transform.position = this.transform.position;
        if (effect.TryGetComponent<WeaponEffectBase>(out var w))
        {
            w.Initialize(target);
            w.IsLeftThrow = isLeftThrow;
        }

        isInstantiated = true;
    }
}