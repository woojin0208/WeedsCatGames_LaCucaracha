using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 저장 데이터 불러오기 패널을 제어한다.
public class LoadPanel : MonoBehaviour
{
    private Image panelImage;

    private void Awake()
    {
        panelImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(ChangeColor());
    }

    // 로딩 패널을 검은색으로 페이드 인한 뒤 숨긴다.
    private IEnumerator ChangeColor()
    {
        float loadTime = 0.55f;
        float elapsedTime = 0;
        Color baseColor = panelImage.color;

        while (elapsedTime < loadTime)
        {
            elapsedTime += Time.deltaTime;
            float alphaValue = Mathf.Lerp(0f, 1f, elapsedTime / loadTime);
            panelImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, alphaValue);

            yield return null;
        }

        panelImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1);
        gameObject.SetActive(false);
    }
}