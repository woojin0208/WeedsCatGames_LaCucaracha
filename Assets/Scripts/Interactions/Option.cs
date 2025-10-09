using UnityEngine;

public class Option : MonoBehaviour, IInteractable
{
    public void Interactive(EntityBase entity)
    {
        UIManager.Instance.OpenPause();
    }
}
