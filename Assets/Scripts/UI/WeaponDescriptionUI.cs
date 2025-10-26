using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 무기 별 첫 획득 시 안내사항 UI
/// </summary>
public class WeaponDescriptionUI : MonoBehaviour
{
    [SerializeField] private Image weaponIconImage;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponDescriptionText;

    private void OnEnable() // NULL INPUT 추가 시 삭제
    {
        Time.timeScale = 0f;
    }

    private void OnDisable() // NULL INPUT 추가 시 삭제
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) gameObject.SetActive(false);
    }

    public void ViewDescription(WeaponBase weapon)
    {
        WeaponDefinition definition = weapon.WeaponDefinition;
        weaponIconImage.sprite = definition.WeaponIcon;
        weaponNameText.text = definition.WeaponName;
        weaponDescriptionText.text = definition.WeaponDescript;
    }

}
