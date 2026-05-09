using System;
using UnityEngine;

// 고정형 보스의 스탯 초기화와 전투 종료 처리를 담당한다.
public class AnchoredBossBase : EntityBase
{
    public event Action OnDamagedAction;

    private EnemyMovement enemyMovement;
    private AnchoredBossRenderer bossRenderer;
    private EnemyAttack enemyAttack;
    private AnchoredBossController bossController;
    [SerializeField] private BossUI bossUI;
    [SerializeField] private BGMPlayer bgmPlayer;
    protected override void Awake()
    {
        base.Awake();

        enemyMovement = GetComponent<EnemyMovement>();
        bossRenderer = GetComponentInChildren<AnchoredBossRenderer>();
        enemyAttack = GetComponentInChildren<EnemyAttack>();
    }

    private void Start()
    {
    }

    private void Initialize()
    {
        if (enemyMovement != null) enemyMovement.Init(stats.GetStat(StatType.MoveSpeed).DefaultValue + stats.GetStat(StatType.MoveSpeed).BonusValue);
        if (enemyAttack != null) enemyAttack.Init(stats.GetStat(StatType.AttackDamage).DefaultValue + stats.GetStat(StatType.AttackDamage).BonusValue);
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        OnDamagedAction?.Invoke();
        Debug.Log("데미지는 받음");

        if (stats.HPStat.DefaultValue <= 0)
        {
            bgmPlayer?.ChangeBGM();

            bossUI?.EndBossBattle();
        }

        bossRenderer.TakeDamaged();
    }

    protected override void TryDie()
    {
        base.TryDie();
    }
    public void ChangeBonusStat(StatType stat, float value)
    {
        stats.GetStat(stat).BonusValue = value;

        Initialize();
    }
}
