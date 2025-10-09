using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI continueHint;
    [SerializeField] private Transform optionsParent;
    [SerializeField] private GameObject optionButtonPrefab;
    [SerializeField] private GameObject textParent;

    /// <summary>
    /// 전체 UI 숨기기
    /// </summary>
    public void Hide() => gameObject.SetActive(false);

    private void OnEnable() => Time.timeScale = 0;
    private void OnDisable() => Time.timeScale = 1;

    /// <summary>
    /// UI 시작 (NPC 기준 위치)
    /// </summary>
    public void Show(Transform target)
    {
        if (target != null)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.position);
            transform.position = screenPoint;
        }
        gameObject.SetActive(true);
    }

    /// <summary>
    /// NPC 대사 한 줄 표시
    /// </summary>
    public void ShowLine(string line)
    {
        if (!textParent.activeSelf) textParent.SetActive(true);
        messageText.text = line.Substring(line.IndexOf(':') + 1).Trim();

        ClearOptions();
        continueHint.gameObject.SetActive(true);
    }

    /// <summary>
    /// 옵션 여러 개 표시
    /// </summary>
    public void ShowOption(List<string> labels, System.Action<int> onSelected)
    {
        ClearOptions();
        HideLine();
        continueHint.gameObject.SetActive(false);

        // 옵션 위치 = 플레이어 머리 위
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

    /// <summary>
    /// 키보드/패드 입력용 옵션 하이라이트
    /// </summary>
    public void HighlightOption(int index)
    {
        /*
        int i = 0;
        foreach (Transform c in optionsParent)
        {
            var txt = c.GetComponentInChildren<TextMeshProUGUI>();
            if (txt == null) continue;

            if (i == index)
            {
                txt.color = Color.yellow;
                txt.fontStyle = FontStyles.Bold;
            }
            else
            {
                txt.color = Color.white;
                txt.fontStyle = FontStyles.Normal;
            }
            i++;
        }
        */
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
