using System;
using UnityEngine;

// 적 스탯 초기화와 피격, 사망 처리를 담당한다.
public class EnemyBase : EntityBase
{
    public event Action OnDamagedAction;

    private EnemyMovement enemyMovement;
    private EnemyRenderer enemyRenderer;
    private EnemyAttack enemyAttack;
    private EnemyController enemyController;

    protected override void Awake()
    {
        base.Awake();

        enemyMovement = GetComponent<EnemyMovement>();
        enemyRenderer = GetComponentInChildren<EnemyRenderer>();
        enemyAttack = GetComponentInChildren<EnemyAttack>();
        enemyController = GetComponent<EnemyController>();
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        enemyMovement.Init(stats.GetStat(StatType.MoveSpeed).DefaultValue + stats.GetStat(StatType.MoveSpeed).BonusValue);
        if (enemyAttack != null) enemyAttack.Init(stats.GetStat(StatType.AttackDamage).DefaultValue + stats.GetStat(StatType.AttackDamage).BonusValue);
        enemyController.Init(stats.GetStat(StatType.AttackSpeed).DefaultValue + stats.GetStat(StatType.AttackSpeed).BonusValue);
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        OnDamagedAction?.Invoke();
        Debug.Log("데미지는 받음");
    }

    protected override void TryDie()
    {
        base.TryDie();

        enemyController.ChangeState(new EnemyDieState());
    }
    public void ChangeBonusStat(StatType stat, float value)
    {
        stats.GetStat(stat).BonusValue = value;

        Initialize();
    }
}
