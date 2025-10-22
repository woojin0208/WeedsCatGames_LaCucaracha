using System;
using UnityEngine;

public class PlayerBase : EntityBase
{
    public event Action OnHitAction;
   
    private PlayerMovement playerMovement;
    private PlayerController playerController;
    private PlayerAttack playerAttack;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerController = GetComponent<PlayerController>();
        playerAttack = GetComponentInChildren<PlayerAttack>();
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

            targetPos.x += -pipe.XOffset; // pipe Ãß°¡ offset
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

        //Debug.Log((stats.GetStat(StatType.AttackDamage).DefaultValue + stats.GetStat(StatType.AttackDamage).BonusValue));
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        OnHitAction?.Invoke();
    }
}
