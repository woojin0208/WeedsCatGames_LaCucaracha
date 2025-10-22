using UnityEngine;

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
        //Debug.Log(transform.position);
        if (effect.TryGetComponent<WeaponEffectBase>(out var w))
        {
            w.Initialize(target);
            w.IsLeftThrow = isLeftThrow;
        }

        isInstantiated = true;
    }
}

