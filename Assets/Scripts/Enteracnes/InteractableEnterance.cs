using UnityEngine;

// 상호작용 입력으로 씬 전환을 시작하는 입장 지점이다.
public class InteractableEnterance : Enterance, IInteractable
{
    [field: SerializeField] public Transform InteractivePos { get; set; }
    public virtual void Interactive(PlayerBase player)
    {
        if (player == null)
        {
            Debug.LogWarning("[InteractableEnterance] player 가 null 입니다.", this);
            return;
        }

        EnterArea(EnteranceType.Interactable);
    }

}