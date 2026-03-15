using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 대화창 UI 표시를 담당한다.
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI continueHint;
    [SerializeField] private Transform optionsParent;
    [SerializeField] private GameObject optionButtonPrefab;
    [SerializeField] private GameObject textParent;

    public void Hide() => gameObject.SetActive(false);

    private void OnEnable() => Time.timeScale = 0;
    private void OnDisable() => Time.timeScale = 1;

    // 대상 위치 기준으로 대화 UI를 표시한다.
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
        messageText.text = line.Substring(line.IndexOf(':') + 1).Trim();

        ClearOptions();
        continueHint.gameObject.SetActive(true);
    }

    // 선택지 목록을 생성하고 표시한다.
    public void ShowOption(List<string> labels, System.Action<int> onSelected)
    {
        ClearOptions();
        HideLine();
        continueHint.gameObject.SetActive(false);

        var playerPos = PlayerManager.Instance.PlayerTextPosition;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(playerPos.position);
        optionsParent.position = screenPoint;

        for (int i = 0; i < labels.Count; i++)
        {
            var btnGo = Instantiate(optionButtonPrefab, optionsParent);
            var btn = btnGo.GetComponent<Button>();
            var txt = btnGo.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = labels[i];

            int idx = i;
            btn.onClick.AddListener(() => onSelected(idx));
        }
    }

    // 키보드 또는 패드 입력용 선택지 하이라이트를 처리한다.
    public void HighlightOption(int index)
    {
    }

    private void HideLine() => textParent.SetActive(false);

    private void ClearOptions()
    {
        foreach (Transform c in optionsParent)
        {
            Destroy(c.gameObject);
        }
    }
}