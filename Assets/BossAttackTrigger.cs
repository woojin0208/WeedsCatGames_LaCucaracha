using UnityEngine;

public class BossAttackTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.TryGetComponent<PlayerBase>(out var player);

            if (player != null) player.TakeDamage(5);
        }
    }
}
