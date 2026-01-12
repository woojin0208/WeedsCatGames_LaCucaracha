using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationBase : MonoBehaviour
{
    protected SpriteRenderer entityRenderer;
    protected Animator animator;

    [SerializeField]
    private Color hitColor;

    protected Color originColor;
    private Coroutine hitFlashCoroutine;

    public bool IsLeft { get; protected set; } = false;

    public event Action OnDieAction;
    MaterialPropertyBlock mpb;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        entityRenderer = GetComponent<SpriteRenderer>();
        originColor = entityRenderer.color;
        mpb = new MaterialPropertyBlock();
    }


    /// <summary>
    /// Renderer 방향 전환 및 Animator 파라미터 전달
    /// </summary>
    /// <param name="x"></param>
    public virtual void WalkAnim(float x)
    {
        //if (x != 0) playerSprite.flipX = x < 0 ? true : false;
        if (x != 0) IsLeft = x < 0 ? true : false;

        Vector3 xScale = transform.localScale;
        xScale.x = IsLeft ? 1 : -1;
        transform.localScale = xScale;

        animator.SetFloat("XDirection", Mathf.Abs(x));
    }
    /// <summary>
    /// attackNum : 콤보 또는 SpecialAttack Number
    /// </summary>
    /// <param name="attackNum"></param>
    public virtual void AttackAnim(int attackNum) 
    {
        //Debug.Log($"attackNum == {attackNum}");
        animator.SetInteger("AttackNum", attackNum);
    }

    /// <summary>
    /// 1 : Attack Timing / 2 : EndAttackTiming / 0 : EndAttack
    /// </summary>
    /// <param name="isAttackTiming"></param>
    public virtual void OnAttackTiming(int isAttackTiming)
    {
        //Debug.Log(isAttackTiming);
        OnAttackEvent?.Invoke(isAttackTiming);
    }

    public void OnDieTiming()
    {
        OnDieAction?.Invoke();
    }
    public virtual void TakeDamaged()
    {
        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
        }

        hitFlashCoroutine = StartCoroutine(FlashHit());
    }

    private IEnumerator FlashHit()
    {
        float t = 0f, dur = 0.2f;
        while (t < dur)
        {
            entityRenderer.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", hitColor);      // Sprites/Default / URP 2D 호환
            entityRenderer.SetPropertyBlock(mpb);
            t += Time.deltaTime;
            yield return null;                     // 매 프레임 재적용
        }
        entityRenderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", originColor);
        entityRenderer.SetPropertyBlock(mpb);
    }

    public virtual void DieAnim()
    {
        animator.SetBool("IsDie", true);
    }

    public event Action<int> OnAttackEvent;
}
