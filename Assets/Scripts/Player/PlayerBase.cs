using System;
using UnityEngine;

// 플레이어 기본 능력치와 피격 처리를 담당한다.
public class PlayerBase : EntityBase
{
    public event Action OnHitAction;
   
    private PlayerMovement playerMovement;
    private PlayerController playerController;
    private PlayerAttack playerAttack;
    private PlayerRenderer playerRenderer;
    protected override void Awake()
    {
        base.Awake();

        playerMovement = GetComponent<PlayerMovement>();
        playerController = GetComponent<PlayerController>();
        playerAttack = GetComponentInChildren<PlayerAttack>();
        playerRenderer = GetComponentInChildren<PlayerRenderer>();
        InitializeStats();
    }

    private void Start()
    {
        SpawnPlayer();

        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController != null)
            cameraController.SetPlayer(transform);
    }

    private void SpawnPlayer()
    {
        Enterance currentEnterance = PlayerManager.Instance.CurrentEnterance;
        Vector3 currentSpawnPoint = currentEnterance.transform.position;

        if (currentSpawnPoint != null) transform.position = currentSpawnPoint;

        if (currentEnterance.GetType() == typeof(PipeEnterance))
        {
            PipeEnterance pipe = (PipeEnterance)currentEnterance;

            Vector3 targetPos = pipe.transform.position;

            // 파이프 시작 위치에 맞춰 X축 보정값을 적용한다.
            targetPos.x += -pipe.XOffset;
            transform.position = targetPos;

            playerController.ChangeState(new PlayerPipeWarpState(false, pipe.IsLeftStart));
        }
        else playerController.ChangeState(new PlayerIdleState());
    }

    public void InitializeStats()
    {
        playerMovement.InitStats(stats.GetStat(StatType.MoveSpeed).DefaultValue,
                             stats.GetStat(StatType.DashSpeed).DefaultValue,
                             stats.GetStat(StatType.JumpForce).DefaultValue);

        playerAttack.InitStats(stats.GetStat(StatType.AttackDamage).DefaultValue + stats.GetStat(StatType.AttackDamage).BonusValue);
    }

    public override void TakeDamage(float damage)
    {
        if (playerMovement != null && playerMovement.IsDashing) return;

        base.TakeDamage(damage);
        OnHitAction?.Invoke();

        playerRenderer.TakeDamaged();
    }
}
