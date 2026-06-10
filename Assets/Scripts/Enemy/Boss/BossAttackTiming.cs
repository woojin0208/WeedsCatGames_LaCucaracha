using UnityEngine;

// Boss Animation Event 시점에 공격 판정 콜라이더 활성화
public class BossAttackTiming : MonoBehaviour
{
    [SerializeField] private GameObject[] hitColliders;

    // Animation Event에서 호출.
    public void AttackTiming(int index)
    {
        if (hitColliders == null || hitColliders.Length == 0) return;

        if (index < 0 || index >= hitColliders.Length)
        {
            Debug.LogWarning($"[BossAttackTiming] HitCollider index 가 범위를 벗어났습니다. index = {index}", this);
            return;
        }

        GameObject hitCollider = hitColliders[index];
        if (hitCollider == null)
        {
            Debug.LogWarning($"[BossAttackTiming] HitCollider 가 null 입니다. index = {index}", this);
            return;
        }

        hitCollider.SetActive(true);
    }
}
