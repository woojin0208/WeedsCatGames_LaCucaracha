using UnityEngine;

// 효과를 발생시키는 무기의 파괴 동작을 처리한다.
public class EffectableWeapon : MonoBehaviour
{
    [SerializeField] private GameObject effectPrefab;

    private bool isInstantiated = false;

    public void OnDestruction(WeaponEffectContext context)
    {
        if (isInstantiated) return;

        if (effectPrefab == null)
        {
            Debug.LogWarning("[EffectableWeapon] effectPrefab이 비어 있습니다.", this);
            return;
        }

        GameObject effectObject = Instantiate(effectPrefab, context.HitPosition, Quaternion.identity);

        if (effectObject.TryGetComponent<IWeaponEffect>(out IWeaponEffect weaponEffect))
        {
            weaponEffect.InitializeFromWeaponHit(context);
        }

        isInstantiated = true;
    }
}
