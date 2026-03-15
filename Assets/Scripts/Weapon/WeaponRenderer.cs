using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 무기 렌더링과 애니메이션을 담당한다.
public class WeaponRenderer : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnDestruction()
    {
        audioSource.Play();
        if (animator.enabled == true)
            animator.SetTrigger("Destruction");
    
        Destroy(this.gameObject, 1);
    }
}