using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 메뉴 선택 UI의 강조 상태를 처리한다.
public class MenuSelectableUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image selectedBar;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectedBar != null)
            selectedBar.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectedBar != null)
            selectedBar.gameObject.SetActive(false);
    }
}