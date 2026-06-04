using System.Collections.Generic;
using UnityEngine;

// 상태 이상 효과의 적용, 유지, 해제를 처리한다.
public class EffectBase : MonoBehaviour
{
    private const float WallCheckRadius = 0.1f;

    public StatusEffectData[] Effects;

    private readonly HashSet<IStatusEffectHandler> effectHandlers = new();

    private StatusEffectData[] runtimeEffects;
    private float remainingDuration;

    private void Awake()
    {
        if (Effects == null || Effects.Length == 0 || Effects[0] == null)
        {
            Debug.LogWarning("[EffectBase] Effects가 비어 있습니다.", this);
            enabled = false;
            return;
        }

        runtimeEffects = CreateRuntimeEffects();
        remainingDuration = runtimeEffects[0].duration;
    }

    private void Start()
    {
        ApplyNearbyWallContext();
    }

    private void Update()
    {
        remainingDuration -= Time.deltaTime;

        if (remainingDuration <= 0) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryApplyTo(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryApplyTo(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<IStatusEffectHandler>(out IStatusEffectHandler handler)) return;
        if (!effectHandlers.Remove(handler)) return;

        IgnoreEffect(handler);
    }

    private void OnDisable()
    {
        foreach (IStatusEffectHandler handler in effectHandlers) IgnoreEffect(handler);

        effectHandlers.Clear();
    }

    private StatusEffectData[] CreateRuntimeEffects()
    {
        StatusEffectData[] clones = new StatusEffectData[Effects.Length];

        for (int i = 0; i < Effects.Length; i++)
        {
            if (Effects[i] == null) continue;

            StatusEffectData clone = Instantiate(Effects[i]);
            clone.effect = this;
            clones[i] = clone;
        }

        return clones;
    }

    private void TryApplyTo(Collider2D collision)
    {
        if (!collision.TryGetComponent<IStatusEffectHandler>(out IStatusEffectHandler handler)) return;
        if (!effectHandlers.Add(handler)) return;

        ApplyEffect(handler);
    }

    private void ApplyEffect(IStatusEffectHandler handler)
    {
        foreach (StatusEffectData effect in runtimeEffects)
        {
            if (effect == null) continue;

            effect.duration = remainingDuration;
            handler.ApplyEffect(effect);
        }
    }

    public void IgnoreEffect(IStatusEffectHandler handler)
    {
        foreach (StatusEffectData effect in runtimeEffects)
        {
            if (effect == null) continue;

            handler.IgnoreEffect(effect);
        }
    }

    private void ApplyNearbyWallContext()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, WallCheckRadius);
        Collider2D nearestWall = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag(GameTags.Wall)) continue;

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance >= nearestDistance) continue;

            nearestDistance = distance;
            nearestWall = hit;
        }

        if (nearestWall == null) return;

        float xDirection = transform.position.x > nearestWall.transform.position.x ? -1f : 1f;

        foreach (StatusEffectData effect in runtimeEffects)
        {
            if (effect == null) continue;

            effect.xDir = xDirection;
        }
    }
}
