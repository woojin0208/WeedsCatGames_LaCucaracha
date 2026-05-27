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

        BindCamera();
    }

    private void SpawnPlayer()
    {
        PlayerManager playerManager = PlayerManager.Instance;
        if (playerManager == null)
        {
            Debug.LogWarning("[PlayerBase] PlayerManager 가 null 입니다.", this);
            ChangeToIdleState();
            return;
        }

        Enterance currentEnterance = playerManager.CurrentEnterance;
        if (currentEnterance == null)
        {
            Debug.LogWarning("[PlayerBase] CurrentEnterance 가 null 입니다.", this);
            ChangeToIdleState();
            return;
        }

        if (currentEnterance is PipeEnterance pipeEnterance)
        {
            SpawnFromPipe(pipeEnterance);
            return;
        }

        transform.position = currentEnterance.transform.position;
        ChangeToIdleState();
    }

    private void SpawnFromPipe(PipeEnterance pipeEnterance)
    {
        Vector3 targetPos = pipeEnterance.transform.position;

        targetPos.x += -pipeEnterance.XOffset;
        transform.position = targetPos;

        if (playerController == null)
        {
            Debug.LogWarning("[PlayerBase] PlayerController 가 null 입니다.", this);
            return;
        }

        playerController.ChangeState(new PlayerPipeWarpState(false, pipeEnterance.IsLeftStart));
    }

    private void BindCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("[PlayerBase] MainCamera 가 null 입니다.", this);
            return;
        }

        if (mainCamera.TryGetComponent(out CameraController cameraController))
        {
            cameraController.SetPlayer(transform);
        }
    }

    private void ChangeToIdleState()
    {
        if (playerController == null)
        {
            Debug.LogWarning("[PlayerBase] PlayerController 가 null 입니다.", this);
            return;
        }

        playerController.ChangeState(new PlayerIdleState());
    }

    public void InitializeStats()
    {
        playerMovement.InitStats(
            stats.GetStat(StatType.MoveSpeed).DefaultValue,
                             stats.GetStat(StatType.DashSpeed).DefaultValue,
                             stats.GetStat(StatType.JumpForce).DefaultValue);

        playerAttack.InitStats(
            stats.GetStat(StatType.AttackDamage).DefaultValue + stats.GetStat(StatType.AttackDamage).BonusValue);
    }

    public override void TakeDamage(float damage)
    {
        if (playerMovement != null && playerMovement.IsDashing) return;

        base.TakeDamage(damage);
        OnHitAction?.Invoke();

        playerRenderer.TakeDamaged();
    }
}
