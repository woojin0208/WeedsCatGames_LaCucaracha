using System;
using UnityEngine;

public class PlayerBase : EntityBase
{
    public event Action OnHitAction;

    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponentInChildren<PlayerAttack>();
        InitializeStats();

    }

    private void Start()
    {
        if (PlayerManager.Instance.CurrentSpawnPoint != null) transform.position = PlayerManager.Instance.CurrentSpawnPoint;
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController != null)
            cameraController.SetPlayer(transform);
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
