using UnityEngine;
using UnityEngine.EventSystems;

// 인벤토리 슬롯 아이콘 표시를 담당한다.
public class InventoryIcon : MonoBehaviour
{
    private string instanceId;
    
    public void SetWeapon(WeaponBase weaponBase, string instanceId, int durability)
    {
        this.instanceId = instanceId;
    }

    public void OnSelect()
    {
        if (string.IsNullOrWhiteSpace(instanceId)) return;

        PlayerManager.Instance?.SelectWeapon(instanceId);
    }

    private void OnDisable()
    {
        Destroy(gameObject);    
    }

    public void OnPointerClick(BaseEventData eventData)
    {
        PointerEventData pointerData = eventData as PointerEventData;
        if (pointerData == null) return;

        if (pointerData.button == PointerEventData.InputButton.Left) OnSelect();
    }
}