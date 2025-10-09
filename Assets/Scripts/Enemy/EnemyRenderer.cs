using System;
using System.Collections;
using UnityEngine;
public class EnemyRenderer : AnimationBase
{   
    public event Action EndHitEvent;

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
        base.TakeDamaged();
        animator.SetTrigger("DamagedTrigger");
    }

    public void BlindAnim()
    {
        animator.SetFloat("XDirection", 0);
    }

}
