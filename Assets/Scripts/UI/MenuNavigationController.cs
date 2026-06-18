using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuNavigationController : MonoBehaviour
{
    [SerializeField] private Selectable[] menuItems;
    [SerializeField] private int defaultIndex;

    private int currentIndex = 0;

    private void OnEnable()
    {
        SubscribeInput(true);

        SelectIndex(defaultIndex);
    }

    private void OnDisable()
    {
        SubscribeInput(false);
    }

    private void SubscribeInput(bool isSub)
    {
        InputStateManager inputStateManager = InputStateManager.Instance;
        if (inputStateManager == null) return;

        if (isSub)
        {
            inputStateManager.PauseUpPressedRequested -= MoveUp;
            inputStateManager.PauseDownPressedRequested -= MoveDown;
            inputStateManager.PauseSubmitPressedRequested -= SubmitItem;

            inputStateManager.PauseUpPressedRequested += MoveUp;
            inputStateManager.PauseDownPressedRequested += MoveDown;
            inputStateManager.PauseSubmitPressedRequested += SubmitItem;
        }
        else
        {
            inputStateManager.PauseUpPressedRequested -= MoveUp;
            inputStateManager.PauseDownPressedRequested -= MoveDown;
            inputStateManager.PauseSubmitPressedRequested -= SubmitItem;
        }
    }


    public void SelectItem(Selectable item)
    {
        if (item == null) return;

        int index = Array.IndexOf(menuItems, item);
        if (index < 0) return;

        SelectIndex(index);
    }

    private void SelectIndex(int index)
    {
        if (menuItems == null || menuItems.Length == 0) return;

        CancelSelectItem();

        currentIndex = Mathf.Clamp(index, 0, menuItems.Length - 1);
        if (!IsValidIndex(currentIndex)) return;

        GameObject currentItem = menuItems[currentIndex].gameObject;
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(currentItem);
        }

        MenuSelectableUI selectableUI = currentItem.GetComponent<MenuSelectableUI>();
        if (selectableUI != null)
            selectableUI.SetSelected(true);

    }

    private void CancelSelectItem()
    {
        if (menuItems == null || menuItems.Length == 0) return;
        if (!IsValidIndex(currentIndex)) return;

        GameObject beforeItem = menuItems[currentIndex].gameObject;

        MenuSelectableUI selectableUI = beforeItem.GetComponent<MenuSelectableUI>();
        if (selectableUI != null)
        {
            selectableUI.SetSelected(false);
        }
    }

    private void MoveUp() => MoveSelection(-1);
    private void MoveDown() => MoveSelection(1);

    private void MoveSelection(int direction)
    {
        SelectIndex(currentIndex + direction);
    }

    private void SubmitItem()
    {
        if (EventSystem.current == null) return;

        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null) return;

        Button itemButton = selected.GetComponent<Button>();
        if (itemButton == null || !itemButton.interactable) return;
        itemButton.onClick.Invoke();
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < menuItems.Length && menuItems[index] != null;
    }
}
