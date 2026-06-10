using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 보스 체력 UI와 전투 종료 화면 전환을 처리한다.
public class BossUI : MonoBehaviour
{
    private const float BossNameFadeDuration = 1f;
    private const float EndGameDelay = 2f;
    private const float RestartDelay = 4f;

    [SerializeField] private TextMeshProUGUI bossName;
    [SerializeField] private GameObject hp;
    [SerializeField] private Image hpSlider;

    [SerializeField] private AnchoredBossBase boss;

    [SerializeField] private Image blackScreen;
    [SerializeField] private Image endScreen;

    private Coroutine bossNameCoroutine;
    private Coroutine endSequenceCoroutine;
    private bool isBattleEnded;

    public void StartBossBattle()
    {
        if (bossNameCoroutine != null)
        {
            StopCoroutine(bossNameCoroutine);
        }

        isBattleEnded = false;

        if (bossName != null)
        {
            bossName.gameObject.SetActive(true);
            SetBossNameAlpha(1f);
        }

        if (hp != null)
        {
            hp.SetActive(false);
        }

        bossNameCoroutine = StartCoroutine(BossNameRoutine());
    }

    public void EndBossBattle()
    {
        if (isBattleEnded) return;

        isBattleEnded = true;

        if (hp != null)
        {
            hp.SetActive(false);
        }

        if (endSequenceCoroutine != null)
        {
            StopCoroutine(endSequenceCoroutine);
        }

        endSequenceCoroutine = StartCoroutine(EndSequenceRoutine());
    }

    private void OnEnable()
    {
        if (boss != null) boss.OnDamagedAction += UpdateHPSlider;
    }

    private void OnDisable()
    {
        if (boss != null) boss.OnDamagedAction -= UpdateHPSlider;
    }

    private IEnumerator BossNameRoutine()
    {
        float timer = BossNameFadeDuration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / BossNameFadeDuration);
            SetBossNameAlpha(alpha);

            yield return null;
        }

        if (bossName != null)
        {
            bossName.gameObject.SetActive(false);
        }

        ViewHPSlider();
        bossNameCoroutine = null;
    }

    private IEnumerator EndSequenceRoutine()
    {
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(EndGameDelay);

        if (endScreen != null)
        {
            endScreen.gameObject.SetActive(true);   
        }

        yield return new WaitForSeconds(RestartDelay);

        UIManager.Instance?.Restart();
        endSequenceCoroutine = null;
    }

    private void ViewHPSlider()
    {
        if (hp != null) hp.SetActive(true);

        UpdateHPSlider();
    }

    private void UpdateHPSlider()
    {
        if (hpSlider == null || boss == null || boss.stats == null) return;
        if (!hpSlider.gameObject.activeInHierarchy) return;

        float maxHp = boss.stats.HPStat.MaxValue;
        if (maxHp <= 0f) return;

        hpSlider.fillAmount = boss.stats.HPStat.DefaultValue / maxHp;
    }

    private void SetBossNameAlpha(float alpha)
    {
        if (bossName == null) return;

        Color color = bossName.color;
        color.a = alpha;
        bossName.color = color;
    }
}