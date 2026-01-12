using System;
using System.Collections;
using UnityEngine;
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

    /*
    public void OnAttackTiming(int isAttackTiming)
    {
        OnAttackEvenet?.Invoke(isAttackTiming == 1 ? true : false);
    }
    */

    public void EndHitTiming()
    {
        EndHitEvent?.Invoke();
    }
    public void TakeDamage()
    {
        if (vfxPlayer != null) vfxPlayer.StartVFX(hitSoundNum);

        base.TakeDamaged();
        animator.SetTrigger("DamagedTrigger");
    }

    public void BlindAnim()
    {
        animator.SetFloat("XDirection", 0);
    }
}
