using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 씬 전환 로딩 패널의 페이드 인/아웃을 처리한다.
public class LoadPanel : MonoBehaviour
{
    [SerializeField] private float fadeTime = 0.55f;

    private Image panelImage;
    private Coroutine fadeCoroutine;

    public event Action Closed;

    public bool IsShowing { get; private set; }

    private void Awake()
    {
        panelImage = GetComponent<Image>();

        if (panelImage == null)
        {
            Debug.LogWarning("[LoadPanel] Image Component가 없습니다.", this);
            return;
        }

        HideImmediate();
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(0f, 1f, true);
    }

    public IEnumerator FadeOut()
    {
        yield return Fade(1f, 0f, false);
        Closed?.Invoke();
    }

    public void ShowImmediate()
    {
        if (panelImage == null) return;

        gameObject.SetActive(true);
        IsShowing = true;
        SetAlpha(1f);
    }
    public void HideImmediate()
    {
        if (panelImage == null) return;

        SetAlpha(0f);
        IsShowing = false;
        gameObject.SetActive(false);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, bool keepActiveAfterFade)
    {
        if (panelImage == null) yield break;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        gameObject.SetActive(true);
        IsShowing = true;

        float elapsedTime = 0f;
        SetAlpha(startAlpha);

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeTime);
            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(endAlpha);

        if (!keepActiveAfterFade)
        {
            IsShowing = false;
            gameObject.SetActive(false);
        }
    }

    private void SetAlpha(float alpha)
    {
        Color color = panelImage.color;
        color.a = alpha;
        panelImage.color = color;
    }
}