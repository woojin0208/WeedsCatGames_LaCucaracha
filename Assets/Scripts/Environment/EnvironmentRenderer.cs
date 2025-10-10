using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interactive 가능한 환경 사물 / 생물 등의 Animation / Animator / Sprite를 관리하는 Class
/// </summary>
public class EnvironmentRenderer : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Interactive()
    {
        animator.SetTrigger("Interactive");
    }
}
