using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bossName;
    [SerializeField] private GameObject hp;
    [SerializeField] private Image hpSlider;

    [SerializeField] private AnchoredBossBase boss;

    [SerializeField] private Image BlackScreen;
    [SerializeField] private Image EndScreen;

    public void StartBossBattle()
    {
        StartCoroutine(HideBossName());
    }

    private IEnumerator HideBossName()
    {
        float timer = 1f;

        Color targetColor = bossName.color;

        while (timer <= 0)
        {
            targetColor.a = timer;
            timer -= Time.deltaTime;

            bossName.color = targetColor;

            yield return null;
        }

        bossName.gameObject.SetActive(false);

        ViewHPSlider();
    }

    private void ViewHPSlider()
    {
        hp.gameObject.SetActive(true);

        hpSlider.fillAmount = boss.stats.HPStat.DefaultValue / boss.stats.HPStat.MaxValue;
    }

    public void EndBossBattle()
    {
        hp.SetActive(false);

        BlackScreen.gameObject.SetActive(true);

        Invoke(nameof(EndGame), 2);
    }

    private void EndGame()
    {
        EndScreen.gameObject.SetActive(true);

        Invoke(nameof(ResetGame), 4);
    }

    private void ResetGame()
    {
        UIManager.Instance.Restart();
    }
    private void Update()
    {
        if (hpSlider.gameObject.activeSelf) hpSlider.fillAmount = boss.stats.HPStat.DefaultValue / boss.stats.HPStat.MaxValue;
    }
}
