using System;
using UnityEngine;

public class EffectBase : MonoBehaviour
{
    public StatusEffectData[] Effects;

    [SerializeField]
    private float destroyDuration;

    private float durationTime;

    private void Awake()
    {
        durationTime = Effects[0].duration;
        destroyDuration = Effects[0].duration;
    }

    private void Update()
    {
        destroyDuration -= Time.deltaTime;

        if (destroyDuration <= 0) Destroy(this.gameObject);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IStatusEffectHandler>(out var handler))
        {
            foreach (var e in Effects)
            {
                e.duration = destroyDuration;
                handler.ApplyEffect(e);
                Effects[0].duration = durationTime;
            }
        }
    }

}
