using UnityEngine;

// 옵션 오브젝트 상호작용 시 일시정지 UI를 연다.
public class Option : MonoBehaviour, IInteractable
{
    [field: SerializeField] public Transform InteractivePos { get; set; }

    public void Interactive(PlayerBase player)
    {
        UIManager.Instance.OpenPause();
    }
}