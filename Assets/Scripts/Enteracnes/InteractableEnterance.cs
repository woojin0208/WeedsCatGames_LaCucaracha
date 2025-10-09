using UnityEngine;

public class InteractableEnterance : Enterance, IInteractable
{
    public void Interactive(EntityBase entity)
    {
        EnterArea(nextArea);
    }

}
