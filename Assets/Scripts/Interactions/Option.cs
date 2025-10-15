using UnityEngine;

public class Option : MonoBehaviour, IInteractable
{
    [field: SerializeField] public Transform InteractivePos { get; set; }

    public void Interactive(PlayerBase player)
    {
        UIManager.Instance.OpenPause();
    }
}
