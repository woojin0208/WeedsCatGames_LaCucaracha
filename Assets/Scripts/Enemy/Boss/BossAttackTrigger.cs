using UnityEngine;

// Boss 공격 판정에 닿은 플레이어에게 피해를 준다.
public class BossAttackTrigger : MonoBehaviour
{
    [SerializeField] private float damage = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(GameTags.Player)) return;
        if (!collision.TryGetComponent<PlayerBase>(out PlayerBase player)) return;

        player.TakeDamage(damage);
    }
}
