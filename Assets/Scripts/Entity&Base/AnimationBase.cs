using System;
using System.Collections;
using UnityEngine;

public abstract class AnimationBase : MonoBehaviour
{
    protected SpriteRenderer entityRenderer;
    protected Animator animator;

    [SerializeField]
    private Color hitColor = new Color(255, 170, 170);

    protected Color originColor;
    private Coroutine hitFlashCoroutine;

    public bool IsLeft { get; protected set; }

    public event Action OnDieAction;

    protected virtual void Awake()
    {
        entityRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        originColor = entityRenderer.color;
    }

    /// <summary>
    /// Renderer 방향 전환 및 Animator 파라미터 전달
    /// </summary>
    /// <param name="x"></param>
    public void WalkAnim(float x)
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
    public void AttackAnim(int attackNum) 
    {
        //Debug.Log($"attackNum == {attackNum}");
        animator.SetInteger("AttackNum", attackNum);
    }

    /// <summary>
    /// 1 : Attack Timing / 2 : EndAttackTiming / 0 : EndAttack
    /// </summary>
    /// <param name="isAttackTiming"></param>
    public void OnAttackTiming(int isAttackTiming)
    {
        //Debug.Log(isAttackTiming);
        OnAttackEvent?.Invoke(isAttackTiming);
    }

    public void OnDieTiming()
    {
        OnDieAction?.Invoke();
    }
    public void TakeDamaged()
    {
        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
        }

        hitFlashCoroutine = StartCoroutine(FlashHit());
    }

    private IEnumerator FlashHit()
    {
        entityRenderer.color = hitColor;
        yield return new WaitForSeconds(0.2f);
        entityRenderer.color = originColor;
    }

    public void DieAnim()
    {
        animator.SetBool("IsDie", true);
    }

    public event Action<int> OnAttackEvent;
}
