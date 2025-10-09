using UnityEngine;

/// <summary>
/// this = Player Object's Child Transform.
/// Weapons's parent = this.
/// if Scene Change ? PlayerManager.HasWeapons Update
/// 
/// 클래스 분리의도 : PlayerManager Object의 과한 연산 줄이기.
/// </summary>

public class PlayerInventory : MonoBehaviour
{
    private PlayerManager PM;
    //public List<WeaponInstance> HasWeapons { get; private set; } = new List<WeaponInstance>();


    /// <summary>
    /// Scene 시작 시. PlayerManager의 HasWeapon 할당.
    /// </summary>
    private void OnEnable()
    {
        PM = PlayerManager.Instance;
    }


    /// <summary>
    /// Input 을 통해 무기 Number을 받아, 현재 무기 삭제 후 해당 Number의 무기 생성.
    /// </summary>
    /// <param name="weapon"></param>
    public void ChangeWeapon(int weaponNum)
    {
        if (weaponNum < 0 || weaponNum > PM.MaxWeaponCount) return;

        WeaponInstance instance = PM.HasWeapons[weaponNum];
        if (instance == null) return;
        // 이전 weapon 삭제
        WeaponBase[] beforeWeapons = GetComponentsInChildren<WeaponBase>();
        foreach (WeaponBase before in beforeWeapons) before.gameObject.SetActive(false);

        PM.SelectWeapon(instance.Id);
    }
}

