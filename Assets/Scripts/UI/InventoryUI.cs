using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ?몃깽?좊━ UI ?쒖떆? ?좏깮 ?곹깭瑜?愿由ы븳??
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
    private bool slotsReady;

    private void Start()
    {
        EnsureSlots();
        RefreshInventory();
    }

    private void OnEnable()
    {
        if (PM == null) PM = PlayerManager.Instance;

        if (PM != null) PM.OnChangedWeapon += HandleChangeWeapon;

        StartCoroutine(RefreshInventoryNextFrame());
    }

    private void OnDisable()
    {
        if (PM != null) PM.OnChangedWeapon -= HandleChangeWeapon;
    }

    private void EnsureSlots()
    {
        if (slotsReady) return;
        if (PM == null) PM = PlayerManager.Instance;
        if (PM == null) return;

        currentSlots = new Transform[PM.MaxWeaponCount];

        for (int i = 0; i < currentSlots.Length; i++)
        {
            GameObject slotClone = Instantiate(inventorySlot, weaponIconParent);
            TextMeshProUGUI indexText = slotClone.GetComponentInChildren<TextMeshProUGUI>();

            indexText.text = $"{i + 1}";
            currentSlots[i] = slotClone.transform;
        }

        slotsReady = true;
    }

    private IEnumerator RefreshInventoryNextFrame()
    {
        yield return null;
        RefreshInventory();
    }

    private void RefreshInventory()
    {
        EnsureSlots();

        if (!slotsReady || PM == null || PM.HasWeapons.Count < 1) return;

        for (int i = 0; i < currentSlots.Length; i++)
        {
            UpdateSlot(i);
        }

        if (PM.CurrentWeapon == null)
        {
            ClearSelection();
            return;
        }

        int idx = PM.HasWeapons.
            ToList().
            FindIndex(w => w != null && w.Id == PM.CurrentWeapon.InstanceId);
        
        if (idx < 0) return;

        SelectWeapon(idx);
    }

    private void SelectWeapon(int weaponNum)
    {
        if (currentSlots == null || weaponNum < 0 || weaponNum >= currentSlots.Length) return;

        bool select = false;
        foreach (Transform currentSlot in currentSlots)
        {
            if (currentSlot == null) continue;

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

    // 臾닿린 紐⑸줉 蹂寃??ы빆???몃깽?좊━ UI??諛섏쁺?쒕떎.
    private void HandleChangeWeapon(int weaponNum)
    {
        EnsureSlots();
        if (PM == null || currentSlots == null || weaponNum < 0 || weaponNum >= currentSlots.Length)
            return;

        UpdateSlot(weaponNum);

        if (weaponNum >= PM.HasWeapons.Count || PM.HasWeapons[weaponNum] == null)
        {
            RefreshInventory();
            return;
        }

        SelectWeapon(weaponNum);
    }

    private void ClearSelection()
    {
        if (currentSlots == null) return;

        foreach (Transform currentSlot in currentSlots)
        {
            if (currentSlot == null) continue;

            Image slotImage = currentSlot.GetComponent<Image>();
            if (slotImage != null) slotImage.sprite = inventorySprite;
        }

        if (selectWeaponIcon != null)
        {
            selectWeaponIcon.gameObject.SetActive(false);
        }
    }
    private void UpdateSlot(int weaponNum)
    {
        if (PM == null || currentSlots == null || weaponNum < 0 || weaponNum >= currentSlots.Length)
            return;
        if (weaponNum >= PM.HasWeapons.Count) return;
        if (weaponData == null) { Debug.LogWarning("weaponData is null"); return; }

        var slot = currentSlots[weaponNum];
        if (slot == null) return;

        var childIcon = slot.GetComponentInChildren<InventoryIcon>(true);
        var inst = PM.HasWeapons[weaponNum];

        if (inst == null)
        {
            if (childIcon != null) Destroy(childIcon.gameObject);
            return;
        }

        if (!weaponData.TryGetWeaponPrefab(inst.WeaponId, out WeaponBase prefab))
        {
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
        childIcon.SetWeapon(prefab, inst.Id, inst.Durability);
    }
}
