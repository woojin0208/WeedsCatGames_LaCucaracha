using UnityEngine;

// PlayerAttackState 상태를 정의한다.
public class PlayerAttackState : IPlayerState
{
    public bool CanAttack { get; } = false;
    public bool CanJump { get; } = false;
    public bool CanDash { get; } = false;
    public bool CanWalk { get; } = false;
    public bool CanLadder { get; } = false;
    public bool CanClingWall { get; } = false;
    public bool CanPipeWarp { get; } = false;
    private readonly int comboStep;
    private float timer;

    private const int MaxCombo = 3;
    private const float ComboWindow = 0.43f;
    private bool tryCombo = false;

    // 현재 콤보 단계를 초기화한다.
    public PlayerAttackState(int step = 1)
    {
        comboStep = Mathf.Clamp(step, 1, MaxCombo);
    }

    public void EnterState(PlayerController playerController)
    {
        playerController.Anim.Attack(comboStep);
        playerController.Move.Move(playerController.Move.Velocity.x * 0.1f);
        tryCombo = false;
        timer = 0f;
    }

    public void UpdateState(PlayerController playerController)
    {
        timer += Time.deltaTime;
        if (timer < ComboWindow)
        {
            // 콤보 입력 가능 시간 안에 공격 입력이 들어오면 다음 콤보를 예약한다.
            if (playerController.Input.AttackPressed && comboStep < MaxCombo)
            {
                tryCombo = true;
                Debug.Log("콤보 시도");
            }
        }
        else
        {
            if (tryCombo) playerController.ChangeState(new PlayerAttackState(comboStep + 1));
            else playerController.ChangeState(new PlayerIdleState());
        }
    }

    public void ExitState(PlayerController playerController)
    {
        playerController.Anim.Attack(0);
    }
}