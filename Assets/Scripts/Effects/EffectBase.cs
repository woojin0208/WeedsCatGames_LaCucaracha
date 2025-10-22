using System.Collections.Generic;
using UnityEngine;

public class EffectBase : MonoBehaviour
{
    public StatusEffectData[] Effects;

    [SerializeField]
    private float destroyDuration;

    private float durationTime;

    private List<IStatusEffectHandler> effectHandlers = new List<IStatusEffectHandler>();

    private void Awake()
    {
        durationTime = Effects[0].duration;
        destroyDuration = Effects[0].duration;
    }

    private void Start()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);

        foreach (StatusEffectData e in Effects) e.effect = this;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Wall"))
            {
                float xDir = transform.position.x > hit.transform.position.x ? -1 : 1;
                Debug.LogAssertion($"Effect Pos = {transform.position.x} / Wall Pos = {hit.transform.position.x}");
                foreach (StatusEffectData e in Effects) e.xDir = xDir;
            }
        }
    }

    private void Update()
    {
        destroyDuration -= Time.deltaTime;

        if (destroyDuration <= 0) Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IStatusEffectHandler>(out var handler))
        {
            effectHandlers.Add(handler);

            ApplyEffect(handler);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IStatusEffectHandler>(out var handler))
        {
            if (!effectHandlers.Contains(handler))
            {
                effectHandlers.Add(handler);
                ApplyEffect(handler);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IStatusEffectHandler>(out var handler))
        {
            IgnoreEffect(handler);

            effectHandlers.Remove(handler);
        }
    }

    private void OnDisable()
    {
        foreach (var e in Effects)
            foreach (var eh in effectHandlers) eh.IgnoreEffect(e);

        effectHandlers.Clear();
    }
    private void ApplyEffect(IStatusEffectHandler handler)
    {
        foreach (var e in Effects)
        {
            e.duration = destroyDuration;
            handler.ApplyEffect(e);
            Effects[0].duration = durationTime;
        }

    }

    public void IgnoreEffect(IStatusEffectHandler handler)
    {
        foreach (var e in Effects) handler.IgnoreEffect(e);
    }


}
