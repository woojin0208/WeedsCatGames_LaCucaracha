using Unity.VisualScripting;
using UnityEngine;

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
