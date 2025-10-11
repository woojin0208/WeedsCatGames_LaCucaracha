using UnityEngine;

public interface IInteractable
{
    public Transform InteractivePos { get; set; }
    void Interactive(EntityBase entity = null);
}
