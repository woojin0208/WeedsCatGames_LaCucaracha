using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 플레이어 장비와 인벤토리 상태를 관리한다.
public class PlayerManager : MonoBehaviour
{
    public event Action<int> OnChangedWeapon;

    private readonly List<WeaponInstance?> hasWeapons = new();
    public IReadOnlyList<WeaponInstance?> HasWeapons => hasWeapons;

    public WeaponBase? CurrentWeapon { get; set; }
    [field: SerializeField] public WeaponData? WeaponData { get; private set; }

    public string CurrentSceneName { private set; get; }
    public Enterance CurrentEnterance { get; set; }
    public int CurrentSpawnPoint { get; set; } = 0;
    public Transform PlayerTextPosition { set; get; }

    private PlayerController playerController;
    private PlayerInventory playerInventory;

    public int MaxWeaponCount { get; private set; } = 4;
    [field: SerializeField] public string CurrentEquipId { get; private set; }

    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        WeaponData.ResetOptainWeapons();
    }


    public void Init(PlayerController playerController)
    {
        this.playerController = playerController;
        playerInventory = playerController.GetComponentInChildren<PlayerInventory>();

        for (int i = hasWeapons.Count; i < MaxWeaponCount; i++)
        {
            hasWeapons.Add(null);
        }
        if (!string.IsNullOrEmpty(CurrentEquipId))
        {
            WeaponInstance instance = hasWeapons.FirstOrDefault(w => w != null && w.Id == CurrentEquipId);
            if (instance != null)
            {
                GetWeapon(instance);
                return;
            }
            else return;
        }
    }

    /// <summary>
    /// 현재 장착 중인 무기를 갱신한다.
    /// </summary>
    public void SetWeapon(WeaponBase weapon)
    {
        CurrentWeapon = weapon;
    }

    /// <summary>
    /// 저장된 무기 정보를 기반으로 무기 프리팹을 생성하고 장착한다.
    /// </summary>
    public void GetWeapon(WeaponInstance instance)
    {
        var prefab = WeaponData.GetCurrentWeaponData(instance.WeaponName);
        var wb = Instantiate(prefab.gameObject).GetComponent<WeaponBase>();

        wb.BindInstance(instance.Id);
        Debug.Log($"{wb} 의 id : {instance.Id}");
        playerController.GetWeapon(wb);
        
        if (WeaponData.TryRegisterWeapon(wb))
        {
            UIManager.Instance.ShowWeaponDescription(wb);
        }
    }

    /// <summary>
    /// 빈 슬롯에 새 무기를 추가하고 UI를 갱신한다.
    /// </summary>
    public WeaponInstance? AddWeapon(string weaponName, int dur)
    {
        if (weaponName.Contains("(Clone)")) weaponName = weaponName.Substring(0, weaponName.Length - 7);

        int idx = hasWeapons.FindIndex(w => w == null);
        if (idx == -1) return null;

        var inst = new WeaponInstance(weaponName, dur);
        hasWeapons[idx] = inst;

        OnChangedWeapon?.Invoke(idx);

        playerInventory.ChangeWeapon(idx);
        return inst;
    }

    /// <summary>
    /// 무기 내구도 변경 사항을 저장된 목록과 UI에 반영한다.
    /// </summary>
    public bool UpdateWeapon(string id, int dur, int amount)
    {
        int idx = hasWeapons.FindIndex(w => w != null && w.Id == id);
        if (idx < 0) return false;

        var w = hasWeapons[idx];
        w.Durability += amount;

        if (w.Durability <= 0)
        {
            hasWeapons[idx] = null;
        }
        else
        {
            hasWeapons[idx] = w;
        }

        OnChangedWeapon?.Invoke(idx);
        return true;

    }

    /// <summary>
    /// 무기를 목록과 현재 장착 상태에서 제거한다.
    /// </summary>
    public bool RemoveWeapon(string id, bool isThrow = true)
    {
        int idx = hasWeapons.FindIndex(w => w != null && w.Id == id);
        if (idx < 0) return false;

        hasWeapons[idx] = null;

        OnChangedWeapon?.Invoke(idx);

        CurrentWeapon = null;
        CurrentEquipId = null;

        if (!isThrow) playerController.RemoveWeapon();
        return true;

    }


    public void SetCurrentScene(string sceneName, int spawnPoint)
    {
        CurrentSpawnPoint = spawnPoint;
        Debug.Log("Player Manager 에 저장 " + CurrentEnterance);
        CurrentSceneName = sceneName;
    }

    

    public void SelectWeapon(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        int idx = hasWeapons.FindIndex(w => w != null && string.Equals(w.Id, id));
        if (idx < 0) return;
        Debug.Log("Select Weapon 성공");
        GetWeapon(hasWeapons[idx]);
        CurrentEquipId = id;
        OnChangedWeapon?.Invoke(idx);
    }
}
