using System;
using UnityEngine;

public class AnchoredBossBase : EntityBase
{
    public event Action OnDamagedAction;

    private EnemyMovement enemyMovement;
    private AnchoredBossRenderer bossRenderer;
    private EnemyAttack enemyAttack;
    private AnchoredBossController bossController;
    [SerializeField] private BossUI bossUI;
    [SerializeField] private BGMPlayer bgmPlayer;
    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        bossRenderer = GetComponentInChildren<AnchoredBossRenderer>();
        enemyAttack = GetComponentInChildren<EnemyAttack>();
        //bossController = GetComponent<EnemyController>();
    }

    private void Start()
    {
        //Initialize();
    }

    private void Initialize()
    {
        if (enemyMovement != null) enemyMovement.Init(stats.GetStat(StatType.MoveSpeed).DefaultValue + stats.GetStat(StatType.MoveSpeed).BonusValue);
        if (enemyAttack != null) enemyAttack.Init(stats.GetStat(StatType.AttackDamage).DefaultValue + stats.GetStat(StatType.AttackDamage).BonusValue);
        //bossController.Init(stats.GetStat(StatType.AttackSpeed).DefaultValue + stats.GetStat(StatType.AttackSpeed).BonusValue);
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        OnDamagedAction?.Invoke();
        Debug.Log("데미지는 받음");
        //enemyRenderer.TakeDamage();

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

        //bossController.ChangeState(new EnemyDieState());
        //enemyRenderer.DieAnim();
    }
    public void ChangeBonusStat(StatType stat, float value)
    {
        stats.GetStat(stat).BonusValue = value;

        Initialize();
    }
}
