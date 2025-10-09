using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    /// <summary>
    /// 진짜 로딩이 필요할 때, 넉넉한 시간을 여기 쥐어주고, 이 시간이 지나면 씬을 보여주기.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangeColor()
    {
        float loadTime = 0.55f;
        float elapsedTime = 0;
        Color baseColor = panelImage.color; // 검정색

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
