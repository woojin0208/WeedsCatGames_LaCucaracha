using UnityEngine;

// 상태 객체가 구현해야 하는 공통 진입, 갱신, 종료 규약을 정의한다.
public interface IState<TContext>
{
    void EnterState(TContext context);
    void UpdateState(TContext context);
    void ExitState(TContext context);
}

// 플레이어 상태가 제공해야 하는 전이 가능 조건을 정의한다.
public interface IPlayerState : IState<PlayerController>
{
    bool CanJump { get; }
    bool CanDash { get; }
    bool CanAttack { get; }
    bool CanWalk { get; }
    bool CanLadder { get; }
    bool CanClingWall { get; }

    bool CanPipeWarp { get; }
}

// 적 상태가 제공해야 하는 전이 가능 조건을 정의한다.
public interface IEnemyState : IState<EnemyController>
{ 
    bool CanHitAnim { get; }
    bool CanChase { get; }
}

// 컨텍스트 기반 상태 전이를 처리하는 공통 상태 머신이다.
public abstract class StateMachine<TContext> : MonoBehaviour
{
    public IState<TContext> currentState;

    public void ChangeState(IState<TContext> nextState)
    {
        if (currentState != null)
            currentState.ExitState((TContext)(object)this);

        currentState = nextState;

        currentState.EnterState((TContext)(object)this);
    }

    protected virtual void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState((TContext)(object)this);
        }
    }
}