using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적 사망 상태를 정의한다.
public class EnemyDieState : IEnemyState
{
    public bool CanHitAnim { get; } = false;
    public bool CanChase { get; } = false;

    public void EnterState(EnemyController enemyController)
    {
        enemyController.Move.Idle(0);
        enemyController.Anim.Die(enemyController.GetComponent<EntityBase>());
    }

    public void UpdateState(EnemyController enemyController)
    {
    }

    public void ExitState(EnemyController enemyController)
    {
    }
}