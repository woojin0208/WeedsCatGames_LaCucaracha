using System;
using UnityEngine;

// 일반 적의 피격과 상태 애니메이션을 처리한다.
public class EnemyRenderer : AnimationBase
{
    public event Action EndHitEvent;

    [SerializeField] private VFXPlayer vfxPlayer;
    [SerializeField] private int hitSoundNum = 3;
    [SerializeField] private int dieSoundNum;
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    public void EndHitTiming()
    {
        EndHitEvent?.Invoke();
    }

    public void TakeDamage()
    {
        if (vfxPlayer != null) vfxPlayer.StartVFX(hitSoundNum);

        base.TakeDamaged();
        if (animator != null)
            animator.SetTrigger(AnimatorParams.DamagedTrigger);
    }

    public void BlindAnim()
    {
        if (animator != null)
            animator.SetFloat(AnimatorParams.XDirection, 0);
    }
}