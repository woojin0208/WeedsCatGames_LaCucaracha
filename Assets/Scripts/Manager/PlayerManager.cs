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

        if (WeaponData == null)
        {
            Debug.LogError("[PlayerManager] WeaponData 가 null 입니다.", this);
            return;
        }

        WeaponData.ResetObtainedWeapons();
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
            WeaponInstance instance = hasWeapons.FirstOrDefault(
                w => w != null && w.Id == CurrentEquipId);

            if (instance == null) return;
            
            GetWeapon(instance);
        }
    }

    public bool HasEmptyWeaponSlot()
    {
        return hasWeapons.Any(w => w == null);
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
    public bool GetWeapon(WeaponInstance instance)
    {
        if (instance == null)
        {
            Debug.LogWarning("[PlayerManager] WeaponInstance 가 null 입니다.", this);
            return false;
        }

        if (WeaponData == null)
        {
            Debug.LogError("[PlayerManager] WeaponData 가 null 입니다.", this);
            return false;
        }

        if (!WeaponData.TryGetWeaponPrefab(instance.WeaponId, out WeaponBase prefab)) return false;

        ClearEquippedWeaponObject();

        WeaponBase weapon = Instantiate(prefab.gameObject).GetComponent<WeaponBase>();
        if (weapon == null)
        {
            Debug.LogError($"[PlayerManager] WeaponBase 를 찾을 수 없습니다." +
                $"weaponId: {instance.WeaponId}");
            return false;
        }

        weapon.BindInstance(instance.Id);
        playerController.GetWeapon(weapon);

        if (WeaponData.TryRegisterWeapon(weapon))
        {
            UIManager.Instance?.ShowWeaponDescription(weapon);
        }

        return true;
    }

    /// <summary>
    /// 빈 슬롯에 새 무기를 추가하고 UI를 갱신한다.
    /// </summary>
    public WeaponInstance AddWeapon(WeaponBase weapon)
    {
        if (weapon == null || weapon.WeaponDefinition == null)
        {
            Debug.LogWarning("[PlayerManager] 추가할 무기 정보가 없습니다.", this);
            return null;
        }

        string weaponId = weapon.WeaponDefinition.WeaponId;
        if (string.IsNullOrWhiteSpace(weaponId))
        {
            Debug.LogWarning("[PlayerManager] WeaponId 가 비어 있습니다.", this);
            return null;
        }

        int idx = hasWeapons.FindIndex(w => w == null);
        if (idx == -1) return null;

        WeaponInstance instance = new WeaponInstance(weaponId, weapon.Durability);
        hasWeapons[idx] = instance;

        OnChangedWeapon?.Invoke(idx);

        playerInventory.ChangeWeapon(idx);
        return instance;
    }

    /// <summary>
    /// 무기 내구도 변경 사항을 저장된 목록과 UI에 반영한다.
    /// </summary>
    public bool UpdateWeapon(string id, int amount)
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

        bool isEquippedWeapon = string.Equals(CurrentEquipId, id);
        if (isEquippedWeapon)
        {
            CurrentWeapon = null;
            CurrentEquipId = null;

            if (!isThrow) playerController.RemoveWeapon();
        }

        return true;
    }

    private void ClearEquippedWeaponObject()
    {
        if (playerInventory == null) return;

        WeaponBase[] beforeWeapons = playerInventory.GetComponentsInChildren<WeaponBase>(true);
        for (int i = 0; i < beforeWeapons.Length; i++)
        {
            if (beforeWeapons[i] == null) continue;

            Destroy(beforeWeapons[i].gameObject);
        }
    }

    public void SetCurrentScene(string sceneName, int spawnPoint)
    {
        CurrentSpawnPoint = spawnPoint;
        CurrentSceneName = sceneName;
    }

    public void SelectWeapon(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return;

        int idx = hasWeapons.FindIndex(w => w != null && string.Equals(w.Id, id));
        if (idx < 0) return;
        
        if (!GetWeapon(hasWeapons[idx])) return;
        
        CurrentEquipId = id;
        OnChangedWeapon?.Invoke(idx);
    }
}
