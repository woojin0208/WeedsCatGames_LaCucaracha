using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 대화 UI 표시와 선택지 하이라이트를 처리한다.
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI continueHint;
    [SerializeField] private Transform optionsParent;
    [SerializeField] private GameObject optionButtonPrefab;
    [SerializeField] private GameObject textParent;
    [SerializeField] private Color optionNormalColor = Color.white;
    [SerializeField] private Color optionSelectedColor = new Color(1f, 0.92f, 0.45f, 1f);

    private readonly List<Button> optionButtons = new List<Button>();

    public void Hide() => gameObject.SetActive(false);

    // 대상 위치를 기준으로 대화 UI를 표시한다.
    public void Show(Transform target)
    {
        if (target != null && Camera.main != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.position);
        }

        gameObject.SetActive(true);
    }

    // 현재 대사 한 줄을 표시한다.
    public void ShowLine(string line)
    {
        if (textParent != null && !textParent.activeSelf) textParent.SetActive(true);

        if (messageText != null) messageText.text = line ?? string.Empty;

        ClearOptions();

        if (continueHint != null) continueHint.gameObject.SetActive(true);
    }

    // 선택지 목록을 생성하고 표시한다.
    public void ShowOption(List<string> labels)
    {
        ClearOptions();
        HideLine();

        if (continueHint != null) continueHint.gameObject.SetActive(false);

        if (labels == null || labels.Count == 0) return;

        if (optionsParent == null)
        {
            Debug.LogWarning("[DialogueUI] optionParent 가 null 입니다.", this);
            return;
        }

        if (optionButtonPrefab == null)
        {
            Debug.LogWarning("[DialogueUI] optionButtonPrefab 이 null 입니다.", this);
            return;
        }

        SetOptionsPosition();

        for (int i = 0; i < labels.Count; i++)
        {
            CreateOption(labels[i]);
        }

        HighlightOption(0);
    }

    // 지정 인덱스 선택지를 하이라이트한다.
    public void HighlightOption(int index)
    {
        if (optionButtons.Count == 0) return;

        int clampedIndex = Mathf.Clamp(index, 0, optionButtons.Count - 1);

        for (int i = 0; i < optionButtons.Count; i++)
        {
            Button button = optionButtons[i];
            if (button == null) continue;

            bool isSelected = i == clampedIndex;

            if (button.targetGraphic != null)
            {
                button.targetGraphic.color = isSelected ? optionSelectedColor : optionNormalColor;
            }

            MenuSelectableUI selectableUI = button.GetComponent<MenuSelectableUI>();
            if (selectableUI != null)
            {
                selectableUI.SetSelected(isSelected);
            }
        }

    }

    private void SetOptionsPosition()
    {
        PlayerManager playerManager = PlayerManager.Instance;
        Transform playerTextPosition = playerManager != null ? playerManager.PlayerTextPosition : null;

        if (playerTextPosition == null || Camera.main == null) return;

        optionsParent.position = Camera.main.WorldToScreenPoint(playerTextPosition.position);
    }

    private void CreateOption(string label)
    {
        GameObject buttonObject = Instantiate(optionButtonPrefab, optionsParent);

        Button button = buttonObject.GetComponent<Button>();
        if (button != null)
        {
            button.enabled = false;
            optionButtons.Add(button);

            if (button.targetGraphic != null) button.targetGraphic.color = optionNormalColor;
        }

        TextMeshProUGUI optionText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
        if (optionText != null) optionText.text = label ?? string.Empty;

        MenuSelectableUI selectableUI = buttonObject.GetComponent<MenuSelectableUI>();
        if (selectableUI != null)
        {
            selectableUI.SetPointerEnabled(false);
            selectableUI.SetSelected(false);
        }
    }
    private void HideLine()
    {
        if (textParent != null) textParent.SetActive(false);
    }

    private void ClearOptions()
    {
        optionButtons.Clear();

        if (optionsParent == null) return;

        foreach (Transform child in optionsParent)
        {
            Destroy(child.gameObject);
        }
    }
}
