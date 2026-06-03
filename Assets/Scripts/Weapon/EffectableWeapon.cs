using UnityEngine;

// 효과를 적용할 수 있는 무기 동작을 처리한다.
public class EffectableWeapon : MonoBehaviour
{
    [SerializeField] private GameObject effectPrefab;

    private bool isInstantiated = false;

    public void OnDestruction(WeaponEffectContext context)
    {
        if (isInstantiated) return;

        if (effectPrefab == null)
        {
            Debug.LogWarning("[EffectableWeapon] effectPrefab 이 null 입니다.", this);
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