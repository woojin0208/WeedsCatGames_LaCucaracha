using UnityEngine;
using UnityEngine.EventSystems;

// 인벤토리 슬롯 아이콘 표시를 담당한다.
public class InventoryIcon : MonoBehaviour
{

    private WeaponBase weaponBase;
    public string InstanceID { get; set; }
    private int durability;
    public void SetWeapon(WeaponBase weaponBase, int durability)
    {
        this.weaponBase = weaponBase;
        InstanceID = weaponBase.InstanceId;
        this.durability = durability;
    }
    public void OnSelect()
    {
        Debug.Log(weaponBase.name + durability);
        PlayerManager.Instance.GetWeapon(new WeaponInstance(weaponBase.gameObject.name, durability));

        Debug.Log($"{weaponBase} On Select.");
    }

    public void PutItem()
    {

    }
    private void OnDisable()
    {
        Destroy(gameObject);    
    }

    public void OnPointerClick(BaseEventData eventData)
    {
        var pointerData = (PointerEventData)eventData;

        if (pointerData.button == PointerEventData.InputButton.Left) OnSelect();
    }
}