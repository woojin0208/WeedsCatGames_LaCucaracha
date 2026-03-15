using UnityEngine;

// 플레이어와 상호작용할 수 있는 오브젝트 규약을 정의한다.
public interface IInteractable
{
    public Transform InteractivePos { get; set; }
    void Interactive(PlayerBase player = null);
}