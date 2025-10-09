using UnityEngine;
public interface IState<TContext>
{
    void EnterState(TContext context);
    void UpdateState(TContext context);
    void ExitState(TContext context);
}

public interface IPlayerState : IState<PlayerController>
{
    bool CanJump { get; }
    bool CanDash { get; }
    bool CanAttack { get; }
    bool CanWalk { get; }
    bool CanLadder { get; }
    bool CanClingWall { get; }
}

public interface IEnemyState : IState<EnemyController>
{ 
    bool CanHitAnim { get; }
    bool CanChase { get; }
}

public abstract class StateMachine<TContext> : MonoBehaviour
{
    public IState<TContext> currentState; // Idle State / UpdateState 

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
