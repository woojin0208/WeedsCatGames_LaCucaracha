using UnityEngine;

public class BossAttackTiming : MonoBehaviour
{

    [SerializeField] private GameObject[] hitColliders;
    private void Awake()
    {
    }

    // 애니메이션 이벤트에서 이 함수만 호출하면 됨
    public void AttackTiming(int count)
    {
        hitColliders[count].SetActive(true);
    }
}
