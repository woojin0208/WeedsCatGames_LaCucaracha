using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Setting UI 의 Bar Class. Event Trigger 를 통한 시각 변화
/// </summary>
public class MenuSelectableUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image selectBar;
    [SerializeField] private GameObject selectMark;
    [SerializeField] private TextMeshProUGUI menuText;

    private Color activeBarColor = Color.white;
    private Color inactiveBarColor = new Color(1, 1, 1, 0);

    [SerializeField] private Color activeTextColor;
    [SerializeField] private Color inactiveTextColor;

    private void Awake()
    {
        if (selectBar != null) selectBar.color = inactiveBarColor;

        if (selectMark != null) selectMark.SetActive(false);

        if (menuText != null) menuText.color = inactiveTextColor;
    }

    public void OnPointerEnter(PointerEventData data) => ViewSelectBar();
    public void OnPointerExit(PointerEventData data) => HideSelectBar();
    public void ViewSelectBar()
    {
        if (selectBar != null) selectBar.color = activeBarColor;
        if (selectMark != null) selectMark.SetActive(true);
        if (menuText != null) menuText.color = activeTextColor;
    }

    public void HideSelectBar()
    {
        if (selectBar != null) selectBar.color = inactiveBarColor;
        if (selectMark != null) selectMark.SetActive(false);
        if (menuText != null) menuText.color = inactiveTextColor;
    }
}
