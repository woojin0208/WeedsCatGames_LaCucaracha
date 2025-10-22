using UnityEngine;

public class InteractableEnterance : Enterance, IInteractable
{
    [field: SerializeField] public Transform InteractivePos { get; set; }
    public virtual void Interactive(PlayerBase player)
    {
        EnterArea(nextArea, EnteranceType.Interactable);
    }

}
