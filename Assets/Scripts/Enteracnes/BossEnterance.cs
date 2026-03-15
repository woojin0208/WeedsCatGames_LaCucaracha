using Unity.VisualScripting;
using UnityEngine;

// 보스가 비활성화된 뒤에만 사용할 수 있는 입장 지점이다.
public class BossEnterance : Enterance, IInteractable
{
    [SerializeField] private AnchoredBossBase bossBase;
    [field: SerializeField] public Transform InteractivePos { get; set; }

    private bool canEnter;
    private void Update()
    {
        canEnter = !bossBase.gameObject.activeSelf;
    }
    public virtual void Interactive(PlayerBase player)
    {
        if (!canEnter) return;

        EnterArea(nextArea, EnteranceType.Interactable);
    }

}