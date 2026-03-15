using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 플레이어 상호작용 대상을 탐색하고 실행한다.
public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteractable;
    private IInteractable lastInteractedObject;
    private float nearestInteractDistance = float.MaxValue;

    private PlayerController playerController;
    private PlayerBase playerBase;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        playerBase = GetComponentInParent<PlayerBase>();
    }

    private void Update()
    {
        Vector2 origin = transform.position;
        float radius = 0.3f;
        Vector2 dir = Vector2.zero;
        int layerMask = LayerMask.GetMask("Weapon", "Enterance", "Interactive", "NPC");

        // 범위 안의 상호작용 후보를 모두 수집한다.
        RaycastHit2D[] natural_hits = Physics2D.CircleCastAll(origin, radius, dir, 0, layerMask);
        if (natural_hits == null || natural_hits.Length == 0)
        {
            ClearInteraction();
            return;
        }

        // 동일 콜라이더가 중복 검출된 경우 하나만 남긴다.
        var hits = natural_hits.GroupBy(h => h.collider).Select(g => g.First()).ToArray();

        // 직전에 상호작용한 자기 무기는 후보에서 제외한다.
        var filtered = hits
            .Where(h => !(h.collider.TryGetComponent<WeaponBase>(out var we) && we.GetEntity() == playerBase)).ToArray();

        if (filtered.Length == 0)
        {
            ClearInteraction();
            return;
        }

        // 가장 가까운 대상을 현재 상호작용 대상으로 선택한다.
        var nearest = filtered.OrderBy(h => h.distance).First();

        if (nearest.collider.TryGetComponent<IInteractable>(out var interactable))
        {
            currentInteractable = interactable;
            nearestInteractDistance = nearest.distance;
            UIManager.Instance.CanInteraction(true, nearest.collider.transform);
        }
        else
        {
            ClearInteraction();
        }
    }

    public void TryInteraction()
    {
        if (currentInteractable == null) return;

        currentInteractable.Interactive(playerController.GetComponent<PlayerBase>());
        lastInteractedObject = currentInteractable;
    }

    private void ClearInteraction()
    {
        currentInteractable = null;
        nearestInteractDistance = float.MaxValue;
        UIManager.Instance.CanInteraction(false);
    }
}