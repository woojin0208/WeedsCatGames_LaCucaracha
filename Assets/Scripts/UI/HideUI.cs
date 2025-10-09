using UnityEngine;

public class HideUI : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.gameObject.SetActive(false);
    }
}
