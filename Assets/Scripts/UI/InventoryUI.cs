using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform weaponIconParent;
    [SerializeField] private WeaponData weaponData;

    [SerializeField] private Image weaponIconsPrefab;
    [SerializeField] private Image selectWeaponIcon;
    [SerializeField] private GameObject inventorySlot;

    [SerializeField] private Sprite inventorySprite;
    [SerializeField] private Sprite selectInventorySprite;


    private PlayerManager PM;
    private Transform[] currentSlots = new Transform[] { };

    private void Start()
    {
        currentSlots = new Transform[PM.MaxWeaponCount];

        for (int i = 0; i < currentSlots.Length; i++)
        {
            GameObject slotClone = Instantiate(inventorySlot, weaponIconParent);

            currentSlots[i] = slotClone.transform;
        }

        InitInventory();

    }
    private void OnEnable()
    {
        if (PM == null) PM = PlayerManager.Instance;

        PM.OnChangedWeapon += HandleChangeWeapon;

        GameManager.Instance.SceneChangeAction += HandleChangeScene;
    }

    private void InitInventory()
    {
        for (int i = 0; i < PM.HasWeapons.Count; i++)
        {
            var wi = PM.HasWeapons[i];

            var iconGO = Instantiate(weaponIconsPrefab, currentSlots[i]);
            WeaponBase weaponBase = weaponData.GetCurrentWeaponData(wi.WeaponName);
            Sprite weaponSprite = weaponBase.WeaponSprite;

            if (weaponSprite != null)
            {
                iconGO.sprite = weaponSprite;
                iconGO.TryGetComponent<InventoryIcon>(out var icon);

                icon.SetWeapon(weaponBase, PM.HasWeapons[i].Durability);
            }
            else
                iconGO.enabled = false; // 아이콘 없으면 숨김(선택)

        }
        // 현재 무기 강조
        List<WeaponInstance> hasWeapons = PM.HasWeapons.ToList();

        int idx = hasWeapons.FindIndex(w => w.Id == PM.CurrentWeapon.InstanceId);
        if (idx < 1) return;
        selectWeaponIcon.gameObject.SetActive(true);
        selectWeaponIcon.transform.parent = currentSlots[idx];
    }

    private void SelectWeapon(int weaponNum)
    {
        bool select = false;
        foreach (Transform currentSlot in currentSlots)
        {
            Image slotImage = currentSlot.GetComponent<Image>();

            if (currentSlot == currentSlots[weaponNum])
            {
                slotImage.sprite = selectInventorySprite;

                select = true;
                selectWeaponIcon.gameObject.SetActive(select);
                selectWeaponIcon.transform.parent = currentSlot;
                selectWeaponIcon.rectTransform.anchoredPosition = new Vector2(36, -100);
            }
            else
            {
                slotImage.sprite = inventorySprite;
            }
        }
        selectWeaponIcon.gameObject.SetActive(select);
    }

    /// <summary>
    /// Weapon List 에 변경이 생겼을 때 UI에 변경사항 적용.
    /// </summary>
    /// <param name="weaponNum"></param>
    private void HandleChangeWeapon(int weaponNum)
    {
        if (PM == null || currentSlots == null || weaponNum < 0 || weaponNum >= currentSlots.Length) return;
        if (weaponData == null) { Debug.LogWarning("weaponData is null"); return; }

        var slot = currentSlots[weaponNum];
        if (slot == null) return;

        // 현재 아이콘(있을 수도/없을 수도)
        var childIcon = slot.GetComponentInChildren<InventoryIcon>(true);
        var inst = PM.HasWeapons[weaponNum]; // nullable

        SelectWeapon(weaponNum);
        // 1) 슬롯이 비워진 경우(REMOVE)
        if (inst == null)
        {
            // 아이콘 GameObject 전체 제거(컴포넌트만 파괴 X)
            if (childIcon != null) Destroy(childIcon.gameObject);
            return;
        }

        // 2) ADD/UPDATE
        var prefab = weaponData.GetCurrentWeaponData(inst.WeaponName);
        if (prefab == null)
        {
            Debug.LogWarning($"WeaponData not found for '{inst.WeaponName}'");
            // 안전하게 비우기
            if (childIcon != null) Destroy(childIcon.gameObject);
            return;
        }

        // 아이콘 생성/가져오기
        Image img;
        if (childIcon != null)
        {
            img = childIcon.GetComponent<Image>();
            if (img == null) img = childIcon.gameObject.AddComponent<Image>();
        }
        else
        {
            img = Instantiate(weaponIconsPrefab, slot); // prefab은 Image
            childIcon = img.GetComponent<InventoryIcon>();
            if (childIcon == null) childIcon = img.gameObject.AddComponent<InventoryIcon>();
        }

        // 스프라이트/데이터 바인딩
        var sprite = prefab.WeaponSprite;
        if (sprite == null)
        {
            // 스프라이트 없으면 아이콘 제거
            Destroy(childIcon.gameObject);
            return;
        }

        img.sprite = sprite;
        childIcon.SetWeapon(prefab, inst.Durability);
    }

    private void HandleChangeScene()
    {
        //gameObject.SetActive(false);
    }
}
