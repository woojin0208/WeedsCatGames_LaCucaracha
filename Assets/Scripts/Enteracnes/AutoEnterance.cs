using UnityEngine;

// 충돌 시 자동으로 씬 전환을 시작하는 입장 지점이다.
public class AutoEnterance : Enterance
{
    [SerializeField] private float enterCooldown = 0.2f;

    private bool hasEntered;
    private float enabledTime;

    private void OnEnable()
    {
        hasEntered = false;
        enabledTime = Time.unscaledTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryEnter(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryEnter(collision);
    }

    private void TryEnter(Collider2D collision)
    {
        if (hasEntered) return;
        if (!collision.CompareTag("Player")) return;

        if (Time.unscaledTime - enabledTime < enterCooldown) return;

        hasEntered = EnterArea(EnteranceType.Auto);
    }

}