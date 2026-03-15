using UnityEngine;

// UI 표시 여부를 전환한다.
public class HideUI : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.gameObject.SetActive(false);
    }
}