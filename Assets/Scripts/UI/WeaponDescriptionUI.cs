using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 무기 설명 UI를 표시한다.
public class WeaponDescriptionUI : MonoBehaviour
{
    [SerializeField] private Image weaponIconImage;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponDescriptionText;

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) gameObject.SetActive(false);
    }

    // 무기 정의 데이터를 UI에 표시한다.
    public void ViewDescription(WeaponBase weapon)
    {
        WeaponDefinition definition = weapon.WeaponDefinition;
        weaponIconImage.sprite = definition.WeaponIcon;
        weaponNameText.text = definition.WeaponName;
        weaponDescriptionText.text = definition.WeaponDescript;
    }
}