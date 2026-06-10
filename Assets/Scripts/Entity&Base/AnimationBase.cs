using System;
using System.Collections;
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
    
    private MaterialPropertyBlock mpb;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        entityRenderer = GetComponent<SpriteRenderer>();

        if (entityRenderer != null)
        {
            originColor = entityRenderer.color;
        }

        mpb = new MaterialPropertyBlock();
    }

    public virtual void WalkAnim(float x)
    {
        if (animator == null) return;

        if (x != 0) IsLeft = x < 0 ? true : false;

        Vector3 xScale = transform.localScale;
        xScale.x = IsLeft ? 1 : -1;
        transform.localScale = xScale;

        animator.SetFloat(AnimatorParams.XDirection, Mathf.Abs(x));
    }

    public virtual void AttackAnim(int attackNum)
    {
        if (animator == null) return;

        animator.SetInteger(AnimatorParams.AttackNum, attackNum);
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
        if (entityRenderer == null) return;

        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
        }

        hitFlashCoroutine = StartCoroutine(FlashHit());
    }

    private IEnumerator FlashHit()
    {
        if (entityRenderer == null) yield break;

        float t = 0f;
        float duration = 0.2f;

        while (t < duration)
        {
            entityRenderer.GetPropertyBlock(mpb);
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
        if (animator == null) return;

        animator.SetBool(AnimatorParams.IsDie, true);
    }

    public event Action<int> OnAttackEvent;
}