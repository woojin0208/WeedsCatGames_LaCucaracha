using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public event Action<int> OnChangedWeapon;

    private readonly List<WeaponInstance?> hasWeapons = new();
    public IReadOnlyList<WeaponInstance?> HasWeapons => hasWeapons;

    public WeaponBase? CurrentWeapon { get; set; }
    [field: SerializeField] public WeaponData? WeaponData { get; private set; }

    public string CurrentSceneName { private set; get; }
    /*
    public int CurrentEnterance { private set; get; } = 0;
    public Vector3 CurrentSpawnPoint { set; get; }
    */

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
            WeaponInstance instance = hasWeapons.FirstOrDefault(w => w.Id == CurrentEquipId);
            if (instance != null)
            {
                GetWeapon(instance);
                return;
            }
            else return;
        }
    }
    /// <summary>
    /// 현재 무기 정보를 저장.
    /// 무기 착용 및 변경 시 실행
    /// </summary>
    /// <param name="weapon"></param>
    public void SetWeapon(WeaponBase weapon)
    {
        CurrentWeapon = weapon;
    }

    /// <summary>
    /// 현재 무기를 가져오고 생성.
    /// Scene 전환 시 실행.
    /// </summary>
    /// <param name="playerController"></param>
    public void GetWeapon(WeaponInstance instance)
    {
        var prefab = WeaponData.GetCurrentWeaponData(instance.WeaponName);
        var wb = Instantiate(prefab.gameObject).GetComponent<WeaponBase>();

        wb.BindInstance(instance.Id);              
        Debug.Log($"{wb} 의 id : {instance.Id}");
        playerController.GetWeapon(wb);
        
    }

    /// <summary>
    /// 무기 추가 시 List 할당 후 PlayerManager 업데이트
    /// </summary>
    /// <param name="weaponName"></param>
    /// <param name="dur"></param>
    public WeaponInstance? AddWeapon(string weaponName, int dur)
    {
        if (weaponName.Contains("(Clone)")) weaponName = weaponName.Substring(0, weaponName.Length - 7);

        int idx = hasWeapons.FindIndex(w => w == null);
        if (idx == -1) return null;

        var inst = new WeaponInstance(weaponName, dur);
        hasWeapons[idx] = inst;

        OnChangedWeapon?.Invoke(idx);

        playerInventory.ChangeWeapon(idx);
        return inst; // ★ 반환
    }

    /// <summary>
    /// 무기의 수치 변경 시, List 내 동일 수치 무기의 수치 변경.
    /// </summary>
    /// <param name="weaponName"></param>
    /// <param name="dur"></param>
    /// <param name="amount"></param>
    public bool UpdateWeapon(string id, int dur, int amount)
    {
        int idx = hasWeapons.FindIndex(w => w.Id == id);
        if (idx < 0) return false;

        var w = hasWeapons[idx];
        w.Durability += amount;

        if (w.Durability <= 0)
        {
            hasWeapons[idx] = null;
        }
        else
        {
            hasWeapons[idx] = w; // struct/class 모두 안전
        }

        OnChangedWeapon?.Invoke(idx);
        return true;

    }

    /// <summary>
    /// 무기의 내구도가 다 닳거나, 무기를 내려놓을 시 List 내 동일 수치 무기 제거.
    /// </summary>
    /// <param name="weaponName"></param>
    /// <param name="dur"></param>
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
