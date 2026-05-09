using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 메뉴 선택 UI의 강조 상태를 처리한다.
public class MenuSelectableUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image selectedBar;
    private bool isPointerEnabled = true;

    public void SetSelected(bool isSelected)
    {
        if (selectedBar != null)
            selectedBar.gameObject.SetActive(isSelected);
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
        if (!isPointerEnabled) return;
        SetSelected(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isPointerEnabled) return;
        SetSelected(false);
    }
}
