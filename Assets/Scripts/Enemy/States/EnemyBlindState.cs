using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적 실명 상태를 정의한다.
public class EnemyBlindState : IEnemyState
{
    public bool CanHitAnim { get; } = true;
    public bool CanChase { get; } = false;

    public void EnterState(EnemyController enemyController)
    {
    }

    public void UpdateState(EnemyController enemyController)
    {
    }

    public void ExitState(EnemyController enemyController)
    {
    }
}