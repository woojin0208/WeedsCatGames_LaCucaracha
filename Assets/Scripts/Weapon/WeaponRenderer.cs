using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
