using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 엔티티의 공통 애니메이션과 피격 연출을 관리한다.
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

    public virtual void WalkAnim(float x)
    {
        if (x != 0) IsLeft = x < 0 ? true : false;

        Vector3 xScale = transform.localScale;
        xScale.x = IsLeft ? 1 : -1;
        transform.localScale = xScale;

        animator.SetFloat("XDirection", Mathf.Abs(x));
    }

    public virtual void AttackAnim(int attackNum) 
    {
        animator.SetInteger("AttackNum", attackNum);
    }

    public virtual void OnAttackTiming(int isAttackTiming)
    {
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
            // Sprites/Default와 URP 2D 셰이더 모두 동일한 색상 프로퍼티를 사용한다.
            mpb.SetColor("_Color", hitColor);
            entityRenderer.SetPropertyBlock(mpb);
            t += Time.deltaTime;
            yield return null;
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