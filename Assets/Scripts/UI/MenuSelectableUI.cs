using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 메뉴 선택 UI의 강조 상태를 처리한다.
public class MenuSelectableUI : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private GameObject selectedMark;
    [SerializeField] private Image selectedBar;
    [SerializeField] private TextMeshProUGUI menuText;

    [SerializeField] private Color defaultTextColor = new Color(0.90f, 1f, 0.77f);
    [SerializeField] private Color selectedTextColor = new Color(0.14f, 0.14f, 0.14f);

    private bool isPointerEnabled = true;

    private MenuNavigationController navigationController;
    private void Awake()
    {
        navigationController = GetComponentInParent<MenuNavigationController>();
    }
    private void OnEnable()
    {
        SetSelected(false);
    }
    public void SetSelected(bool isSelected)
    {
        if (selectedMark != null) selectedMark.SetActive(isSelected);
        if (selectedBar != null) selectedBar.color = isSelected ? Color.white : Color.clear;
        if (menuText != null) menuText.color = isSelected ? selectedTextColor : defaultTextColor;
    }

    public void SetPointerEnabled(bool isEnabled)
    {
        isPointerEnabled = isEnabled;
        if (!isPointerEnabled)
        {
            SetSelected(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isPointerEnabled || navigationController == null) return;
        
        Selectable selectable = GetComponent<Selectable>();

        
        navigationController.SelectItem(selectable);
    }
}
