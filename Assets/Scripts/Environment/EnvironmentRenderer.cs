using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 상호작용 가능한 환경 오브젝트의 애니메이션을 재생한다.
public class EnvironmentRenderer : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Interactive()
    {
        animator.SetTrigger("Interactive");
    }
}