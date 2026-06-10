using System;
using UnityEngine;

// 고정형 보스의 스탯 초기화와 전투 종료 처리를 담당한다.
public class AnchoredBossBase : EntityBase
{
    public event Action OnDamagedAction;

    [SerializeField] private BossUI bossUI;
    [SerializeField] private BGMPlayer bgmPlayer;

    private AnchoredBossController bossController;
    private EnemyMovement enemyMovement;
    private AnchoredBossRenderer bossRenderer;
    private EnemyAttack enemyAttack;

    private bool isDeathHandled;

    protected override void Awake()
    {
        base.Awake();

        bossController = GetComponent<AnchoredBossController>();
        enemyMovement = GetComponent<EnemyMovement>();
        bossRenderer = GetComponentInChildren<AnchoredBossRenderer>();
        enemyAttack = GetComponentInChildren<EnemyAttack>();
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (stats == null) return;
        
        Stat moveSpeedStat = stats.GetStat(StatType.MoveSpeed);
        if (enemyMovement != null && moveSpeedStat != null)
        {
            enemyMovement.Init(moveSpeedStat.Value);
        }

        Stat attackDamageStat = stats.GetStat(StatType.AttackDamage);
        if (enemyAttack != null && attackDamageStat != null)
        {
            enemyAttack.Init(attackDamageStat.Value);
        }
    }

    public override void TakeDamage(float damage)
    {
        if (isDeathHandled) return;

        base.TakeDamage(damage);

        OnDamagedAction?.Invoke();

        if (isDeathHandled) return;

        bossRenderer?.TakeDamaged();
    }

    protected override void TryDie()
    {
        if (isDeathHandled) return;

        isDeathHandled = true;

        bgmPlayer?.ChangeBGM();
        bossUI?.EndBossBattle();

        if (bossController != null)
        {
            bossController.Die();
            return;
        }

        bossRenderer?.DieAnim();
    }

    public void ChangeBonusStat(StatType stat, float value)
    {
        if (stats == null) return;

        Stat targetStat = stats.GetStat(stat);
        if (targetStat == null) return;

        targetStat.BonusValue = value;

        Initialize();
    }
}
