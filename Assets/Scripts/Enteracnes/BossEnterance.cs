using UnityEngine;

// 보스가 비활성화된 뒤에만 사용할 수 있는 입장 지점이다.
public class BossEnterance : Enterance, IInteractable
{
    [SerializeField] private AnchoredBossBase bossBase;
    [field: SerializeField] public Transform InteractivePos { get; set; }

    public virtual void Interactive(PlayerBase player)
    {
        if (player == null)
        {
            Debug.LogWarning("[BossEnterance] player 가 null 입니다.", this);
            return;
        }

        if (!CanEnter()) return;

        EnterArea(EnteranceType.Interactable);
    }

    private bool CanEnter()
    {
        if (bossBase == null)
        {
            Debug.LogWarning("[BossEnterance] bossBase 가 null 입니다.", this);
            return false;
        }

        return !bossBase.gameObject.activeInHierarchy;
    }

}