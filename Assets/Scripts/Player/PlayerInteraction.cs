using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        // 1) 모든 히트 가져오기
        RaycastHit2D[] natural_hits = Physics2D.CircleCastAll(origin, radius, dir, 0, layerMask);
        if (natural_hits == null || natural_hits.Length == 0)
        {
            ClearInteraction();
            return;
        }

        // 2) 동일 콜라이더 중복 제거
        var hits = natural_hits.GroupBy(h => h.collider).Select(g => g.First()).ToArray();

        // 3) 마지막 상호작용 객체 제외
        var filtered = hits
            .Where(h => !(h.collider.TryGetComponent<WeaponBase>(out var we) && we.GetEntity() == playerBase)).ToArray();

        if (filtered.Length == 0)
        {
            ClearInteraction();
            return;
        }

        // 4) 가장 가까운 히트 선택
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
