using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 무기 설명 UI를 표시한다.
public class WeaponDescriptionUI : MonoBehaviour
{
    [SerializeField] private Image weaponIconImage;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponDescriptionText;
    [SerializeField] private TextMeshProUGUI weaponDamageText;
    [SerializeField] private TextMeshProUGUI weaponDurabilityText;
    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    // 무기 정의 데이터를 UI에 표시한다.
    public void ViewDescription(WeaponBase weapon)
    {
        if (weapon == null || weapon.WeaponDefinition == null) return;

        WeaponDefinition definition = weapon.WeaponDefinition;

        weaponIconImage.sprite = definition.WeaponIcon;
        weaponNameText.text = definition.WeaponName;
        weaponDescriptionText.text = definition.WeaponDescript;
        weaponDamageText.text = $"공격력 : {FormatNumber(weapon.Damage)}";
        weaponDurabilityText.text = $"내구도 : {weapon.Durability}";
    }

    private string FormatNumber(float value)
    {
        if (Mathf.Approximately(value, Mathf.Round(value)))
        {
            return Mathf.RoundToInt(value).ToString();
        }

        return value.ToString("0.##");
    }


}
