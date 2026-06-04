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
        if (stats == null)
        {
            Debug.LogWarning("[EnemyBase] Stats 가 null 입니다.", this);
            return;
        }

        Stat moveSpeed = stats.GetStat(StatType.MoveSpeed);
        Stat attackDamage = stats.GetStat(StatType.AttackDamage);
        Stat attackSpeed = stats.GetStat(StatType.AttackSpeed);

        if (enemyMovement != null && moveSpeed != null)
        {
            enemyMovement.Init(moveSpeed.Value);
        }

        if (enemyAttack != null && attackDamage != null)
        {
            enemyAttack.Init(attackDamage.Value);
        }

        if (enemyController != null && attackSpeed != null)
        {
            enemyController.Init(attackSpeed.Value);
        }
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
