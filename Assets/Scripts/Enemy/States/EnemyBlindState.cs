using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
