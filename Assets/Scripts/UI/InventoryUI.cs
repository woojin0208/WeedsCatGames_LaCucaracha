using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 인벤토리 UI 표시와 선택 상태를 관리한다.
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform weaponIconParent;
    [SerializeField] private WeaponData weaponData;

    [SerializeField] private Image weaponIconsPrefab;
    [SerializeField] private Image selectWeaponIcon;
    [SerializeField] private GameObject inventorySlot;

    [SerializeField] private Sprite inventorySprite;
    [SerializeField] private Sprite selectInventorySprite;

    [SerializeField] private TextMeshProUGUI slotIndex;
    private PlayerManager PM;
    private Transform[] currentSlots = new Transform[] { };

    private void Start()
    {
        currentSlots = new Transform[PM.MaxWeaponCount];

        for (int i = 0; i < currentSlots.Length; i++)
        {
            GameObject slotClone = Instantiate(inventorySlot, weaponIconParent);
            TextMeshProUGUI indexText = slotClone.GetComponentInChildren<TextMeshProUGUI>();

            indexText.text = $"{i + 1}";
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
        if (PM.HasWeapons.Count < 1) return;
        for (int i = 0; i < PM.HasWeapons.Count; i++)
        {
            var wi = PM.HasWeapons[i];
            var iconGO = Instantiate(weaponIconsPrefab, currentSlots[i]);

            Debug.Log(wi.WeaponName);
#nullable disable
            WeaponBase? weaponBase = weaponData.GetCurrentWeaponData(wi.WeaponName);
            Sprite weaponSprite = weaponBase.WeaponDefinition.WeaponIcon;

            if (weaponSprite != null)
            {
                iconGO.sprite = weaponSprite;
                iconGO.TryGetComponent<InventoryIcon>(out var icon);
                icon.SetWeapon(weaponBase, PM.HasWeapons[i].Durability);
            }
            else
                iconGO.enabled = false;
        }

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

    // 무기 목록 변경 사항을 인벤토리 UI에 반영한다.
    private void HandleChangeWeapon(int weaponNum)
    {
        if (PM == null || currentSlots == null || weaponNum < 0 || weaponNum >= currentSlots.Length) return;
        if (weaponData == null) { Debug.LogWarning("weaponData is null"); return; }

        var slot = currentSlots[weaponNum];
        if (slot == null) return;

        var childIcon = slot.GetComponentInChildren<InventoryIcon>(true);
        var inst = PM.HasWeapons[weaponNum];

        SelectWeapon(weaponNum);
        if (inst == null)
        {
            if (childIcon != null) Destroy(childIcon.gameObject);
            return;
        }

        var prefab = weaponData.GetCurrentWeaponData(inst.WeaponName);
        if (prefab == null)
        {
            Debug.LogWarning($"WeaponData not found for '{inst.WeaponName}'");
            if (childIcon != null) Destroy(childIcon.gameObject);
            return;
        }

        Image img;
        if (childIcon != null)
        {
            img = childIcon.GetComponent<Image>();
            if (img == null) img = childIcon.gameObject.AddComponent<Image>();
        }
        else
        {
            img = Instantiate(weaponIconsPrefab, slot);
            childIcon = img.GetComponent<InventoryIcon>();
            if (childIcon == null) childIcon = img.gameObject.AddComponent<InventoryIcon>();
        }

        var sprite = prefab.WeaponDefinition.WeaponIcon;
        if (sprite == null)
        {
            Destroy(childIcon.gameObject);
            return;
        }

        img.sprite = sprite;
        childIcon.SetWeapon(prefab, inst.Durability);
    }

    private void HandleChangeScene()
    {
    }
}