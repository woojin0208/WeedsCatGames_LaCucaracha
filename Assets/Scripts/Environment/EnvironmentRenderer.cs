using UnityEngine;

// 상호작용 가능한 환경 오브젝트의 애니메이션을 재생한다.
public class EnvironmentRenderer : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Interactive()
    {
        if (animator == null) return;

        animator.SetTrigger(AnimatorParams.Interactive);
    }
}