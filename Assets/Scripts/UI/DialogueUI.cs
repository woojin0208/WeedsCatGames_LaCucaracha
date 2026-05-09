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
    private readonly List<TextMeshProUGUI> optionTexts = new List<TextMeshProUGUI>();

    public void Hide() => gameObject.SetActive(false);

    private void OnEnable() => Time.timeScale = 0f;
    private void OnDisable() => Time.timeScale = 1f;

    // 대상 위치를 기준으로 대화 UI를 표시한다.
    public void Show(Transform target)
    {
        if (target != null)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.position);
            transform.position = screenPoint;
        }

        gameObject.SetActive(true);
    }

    // 현재 대사 한 줄을 표시한다.
    public void ShowLine(string line)
    {
        if (!textParent.activeSelf) textParent.SetActive(true);

        int colonIndex = line != null ? line.IndexOf(':') : -1;
        messageText.text = colonIndex >= 0 ? line.Substring(colonIndex + 1).Trim() : (line ?? string.Empty);

        ClearOptions();
        continueHint.gameObject.SetActive(true);
    }

    // 선택지 목록을 생성하고 표시한다.
    public void ShowOption(List<string> labels)
    {
        ClearOptions();
        HideLine();
        continueHint.gameObject.SetActive(false);

        Transform playerPos = PlayerManager.Instance.PlayerTextPosition;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(playerPos.position);
        optionsParent.position = screenPoint;

        for (int i = 0; i < labels.Count; i++)
        {
            GameObject btnGo = Instantiate(optionButtonPrefab, optionsParent);
            Button btn = btnGo.GetComponent<Button>();
            if (btn != null)
            {
                btn.enabled = false;
            }

            TextMeshProUGUI txt = btnGo.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = labels[i];
            MenuSelectableUI selectableUI = btnGo.GetComponent<MenuSelectableUI>();
            if (selectableUI != null)
            {
                selectableUI.SetPointerEnabled(false);
            }

            if (btn != null)
            {
                optionButtons.Add(btn);
                optionTexts.Add(txt);
                if (btn.targetGraphic != null)
                {
                    btn.targetGraphic.color = optionNormalColor;
                }
            }
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
            bool isSelected = i == clampedIndex;

            if (optionButtons[i].targetGraphic != null)
            {
                optionButtons[i].targetGraphic.color = isSelected ? optionSelectedColor : optionNormalColor;
            }


            MenuSelectableUI selectableUI = optionButtons[i].GetComponent<MenuSelectableUI>();
            if (selectableUI != null)
            {
                selectableUI.SetSelected(isSelected);
            }
        }

    }

    private void HideLine() => textParent.SetActive(false);

    private void ClearOptions()
    {
        optionButtons.Clear();
        optionTexts.Clear();
        foreach (Transform c in optionsParent)
        {
            Destroy(c.gameObject);
        }
    }
}
