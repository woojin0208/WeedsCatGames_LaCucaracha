using UnityEngine;

public class InteractableEnterance : Enterance, IInteractable
{
    [field: SerializeField] public Transform InteractivePos { get; set; }
    public void Interactive(EntityBase entity)
    {
        EnterArea(nextArea);
    }

}
