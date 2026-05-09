using UnityEngine;

// 플레이어 무기 보유 상태와 장착을 관리한다.
public class PlayerInventory : MonoBehaviour
{
    private PlayerManager PM;

    private void OnEnable()
    {
        PM = PlayerManager.Instance;
    }

    // 입력으로 지정한 슬롯의 무기를 장착한다.
    public void ChangeWeapon(int weaponNum)
    {
        if (weaponNum < 0 || weaponNum >= PM.MaxWeaponCount) return;

        WeaponInstance instance = PM.HasWeapons[weaponNum];
        if (instance == null) return;

        WeaponBase[] beforeWeapons = GetComponentsInChildren<WeaponBase>();
        foreach (WeaponBase before in beforeWeapons) before.gameObject.SetActive(false);

        PM.SelectWeapon(instance.Id);
    }
}
